#!/usr/bin/env python

'''
0 - точно фон
1 - точно объект
2 - вероятно фон
3 - вероятно объект

r - сбросить
n - делать GrabCut
'''

import numpy as np
import cv2
import sys

BLUE = [255,0,0]        # цвет области
RED = [0,0,255]         # вероятно фон
GREEN = [0,255,0]       # вероятно объект
BLACK = [0,0,0]         # точно фон
WHITE = [255,255,255]   # точно объект

DRAW_BG = {'color' : BLACK, 'val' : 0}
DRAW_FG = {'color' : WHITE, 'val' : 1}
DRAW_PR_FG = {'color' : GREEN, 'val' : 3}
DRAW_PR_BG = {'color' : RED, 'val' : 2}

# setting up flags
rect = (0,0,1,1)
drawing = False         # выделяем фон или объект
rectangle = False       # выделяем область
rect_over = False       # окончили рисовать область
rect_or_mask = 100      # флаг указываеющий рисуем область или маску
value = DRAW_FG         # по умолчанию рисуем точно объект
thickness = 3           # толщина кисти для рисования

def onmouse(event,x,y,flags,param):
    global img,img2,drawing,value,mask,rectangle,rect,rect_or_mask,ix,iy,rect_over

    # Начинаем рисоват, если правая кнопка нажата, то
    if event == cv2.EVENT_RBUTTONDOWN:
        rectangle = True
        ix,iy = x,y # запоминаем положение начала

    elif event == cv2.EVENT_MOUSEMOVE: # Если двигаем мышь и у нас рисуется область
        if rectangle == True:
            img = img2.copy()   # стираем нарисованную область
            cv2.rectangle(img,(ix,iy),(x,y),BLUE,2) # рисуем прямоугольник
            rect = (ix,iy,abs(ix-x),abs(iy-y))  # сохраняем инфу об области
            rect_or_mask = 0

    elif event == cv2.EVENT_RBUTTONUP: # закончили рисовать
        rectangle = False # завершили рисование
        rect_over = True
        cv2.rectangle(img,(ix,iy),(x,y),BLUE,2) # нарисовали и запомнили область
        rect = (ix,iy,abs(ix-x),abs(iy-y))
        rect_or_mask = 0


    if event == cv2.EVENT_LBUTTONDOWN: # выделяем фон или объект
        if rect_over == False:
            print "Отпустите правую кнопку \n"
        else:
            drawing = True
            cv2.circle(img,(x,y),thickness,value['color'],-1)    # помечаем на изображениии выделенный участки
            cv2.circle(mask,(x,y),thickness,value['val'],-1) # помечаем на маске выделенные участки

    elif event == cv2.EVENT_MOUSEMOVE:
        if drawing == True:
            cv2.circle(img,(x,y),thickness,value['color'],-1)
            cv2.circle(mask,(x,y),thickness,value['val'],-1)

    elif event == cv2.EVENT_LBUTTONUP:
        if drawing == True:
            drawing = False
            cv2.circle(img,(x,y),thickness,value['color'],-1)
            cv2.circle(mask,(x,y),thickness,value['val'],-1)

 if len(sys.argv) == 2:
    filename = sys.argv[1]
else:
    filename = 'C:/im1.jpg'

img = cv2.imread(filename)              # считываем изображение
img2 = img.copy()                               # и сохраняем его копию
mask = np.zeros(img.shape[:2],dtype = np.uint8) # создаем маску, инициализируем нулями - все фон
output = np.zeros(img.shape,np.uint8)           # вывод - тоже нули

# создаем окно ввода и вывода
cv2.namedWindow('output')
cv2.namedWindow('input')
cv2.setMouseCallback('input',onmouse) # привязываем обработчик
# cv2.moveWindow('input',img.shape[1]+10,90)


while(1):
    # В цикле выводи окна
    cv2.imshow('output',output) 
    cv2.imshow('input',img)
    k = 0xFF & cv2.waitKey(1) # ждем клавишу 0хFF для 64 битных машин, ждет 1 мс

    # key bindings
    if k == 27:         # ESC
        break
    elif k == ord('0'): # нажали "0" - рисуем точно фон
        value = DRAW_BG
    elif k == ord('1'): # нажали "1" - рисуем точно объект
        value = DRAW_FG
    elif k == ord('2'): # нажили "2" - рисуем вероятно фон
        value = DRAW_PR_BG
    elif k == ord('3'): # нажали "3" - рисуем вероятно объект
        value = DRAW_PR_FG
    elif k == ord('r'): # все сбросить
        rect = (0,0,1,1) # сбрасываем область
        drawing = False # флаги рисования
        rectangle = False
        rect_or_mask = 100 # сбрасываем флаг операции
        rect_over = False
        value = DRAW_FG # ставим по умолчанию рисование объекта (точно)
        img = img2.copy() # восстанавливаем исходное изображение
        mask = np.zeros(img.shape[:2],dtype = np.uint8) # восстанавливаем маску нулями
        output = np.zeros(img.shape,np.uint8)           # и выходное изображение
    elif k == ord('n'): # выполнить сегментацию
        if (rect_or_mask == 0):         # если сегментация по области
            bgdmodel = np.zeros((1,65),np.float64) # два массива, нужные для алгоритма
            fgdmodel = np.zeros((1,65),np.float64)
            # Здесь передаем само изображение, маску, информацию об области, вспомогательные массивы,
            # Указание о том, что нужно сделать одну итерацию алгоритма, пояснение о том, что алгоритм
            # Должен работать для области
            cv2.grabCut(img2,mask,rect,bgdmodel,fgdmodel,1,cv2.GC_INIT_WITH_RECT)
            rect_or_mask = 1
        elif rect_or_mask == 1:         # сегментация по маске
            bgdmodel = np.zeros((1,65),np.float64)
            fgdmodel = np.zeros((1,65),np.float64)
            cv2.grabCut(img2,mask,rect,bgdmodel,fgdmodel,1,cv2.GC_INIT_WITH_MASK)

    # Преобразуем маску. Знчение 1 и 3 в 255, остальные в 0
    mask2 = np.where((mask==1) + (mask==3),255,0).astype('uint8')
    # Сохраняем в вывод, так чтобы был виден только объект.
    output = cv2.bitwise_and(img2,img2,mask=mask2)

cv2.destroyAllWindows()
