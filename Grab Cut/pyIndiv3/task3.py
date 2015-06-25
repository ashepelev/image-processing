#!/usr/bin/env python

'''
0 - ����� ���
1 - ����� ������
2 - �������� ���
3 - �������� ������

r - ��������
n - ������ GrabCut
'''

import numpy as np
import cv2
import sys

BLUE = [255,0,0]        # ���� �������
RED = [0,0,255]         # �������� ���
GREEN = [0,255,0]       # �������� ������
BLACK = [0,0,0]         # ����� ���
WHITE = [255,255,255]   # ����� ������

DRAW_BG = {'color' : BLACK, 'val' : 0}
DRAW_FG = {'color' : WHITE, 'val' : 1}
DRAW_PR_FG = {'color' : GREEN, 'val' : 3}
DRAW_PR_BG = {'color' : RED, 'val' : 2}

# setting up flags
rect = (0,0,1,1)
drawing = False         # �������� ��� ��� ������
rectangle = False       # �������� �������
rect_over = False       # �������� �������� �������
rect_or_mask = 100      # ���� ������������ ������ ������� ��� �����
value = DRAW_FG         # �� ��������� ������ ����� ������
thickness = 3           # ������� ����� ��� ���������

def onmouse(event,x,y,flags,param):
    global img,img2,drawing,value,mask,rectangle,rect,rect_or_mask,ix,iy,rect_over

    # �������� �������, ���� ������ ������ ������, ��
    if event == cv2.EVENT_RBUTTONDOWN:
        rectangle = True
        ix,iy = x,y # ���������� ��������� ������

    elif event == cv2.EVENT_MOUSEMOVE: # ���� ������� ���� � � ��� �������� �������
        if rectangle == True:
            img = img2.copy()   # ������� ������������ �������
            cv2.rectangle(img,(ix,iy),(x,y),BLUE,2) # ������ �������������
            rect = (ix,iy,abs(ix-x),abs(iy-y))  # ��������� ���� �� �������
            rect_or_mask = 0

    elif event == cv2.EVENT_RBUTTONUP: # ��������� ��������
        rectangle = False # ��������� ���������
        rect_over = True
        cv2.rectangle(img,(ix,iy),(x,y),BLUE,2) # ���������� � ��������� �������
        rect = (ix,iy,abs(ix-x),abs(iy-y))
        rect_or_mask = 0


    if event == cv2.EVENT_LBUTTONDOWN: # �������� ��� ��� ������
        if rect_over == False:
            print "��������� ������ ������ \n"
        else:
            drawing = True
            cv2.circle(img,(x,y),thickness,value['color'],-1)    # �������� �� ������������ ���������� �������
            cv2.circle(mask,(x,y),thickness,value['val'],-1) # �������� �� ����� ���������� �������

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

img = cv2.imread(filename)              # ��������� �����������
img2 = img.copy()                               # � ��������� ��� �����
mask = np.zeros(img.shape[:2],dtype = np.uint8) # ������� �����, �������������� ������ - ��� ���
output = np.zeros(img.shape,np.uint8)           # ����� - ���� ����

# ������� ���� ����� � ������
cv2.namedWindow('output')
cv2.namedWindow('input')
cv2.setMouseCallback('input',onmouse) # ����������� ����������
# cv2.moveWindow('input',img.shape[1]+10,90)


while(1):
    # � ����� ������ ����
    cv2.imshow('output',output) 
    cv2.imshow('input',img)
    k = 0xFF & cv2.waitKey(1) # ���� ������� 0�FF ��� 64 ������ �����, ���� 1 ��

    # key bindings
    if k == 27:         # ESC
        break
    elif k == ord('0'): # ������ "0" - ������ ����� ���
        value = DRAW_BG
    elif k == ord('1'): # ������ "1" - ������ ����� ������
        value = DRAW_FG
    elif k == ord('2'): # ������ "2" - ������ �������� ���
        value = DRAW_PR_BG
    elif k == ord('3'): # ������ "3" - ������ �������� ������
        value = DRAW_PR_FG
    elif k == ord('r'): # ��� ��������
        rect = (0,0,1,1) # ���������� �������
        drawing = False # ����� ���������
        rectangle = False
        rect_or_mask = 100 # ���������� ���� ��������
        rect_over = False
        value = DRAW_FG # ������ �� ��������� ��������� ������� (�����)
        img = img2.copy() # ��������������� �������� �����������
        mask = np.zeros(img.shape[:2],dtype = np.uint8) # ��������������� ����� ������
        output = np.zeros(img.shape,np.uint8)           # � �������� �����������
    elif k == ord('n'): # ��������� �����������
        if (rect_or_mask == 0):         # ���� ����������� �� �������
            bgdmodel = np.zeros((1,65),np.float64) # ��� �������, ������ ��� ���������
            fgdmodel = np.zeros((1,65),np.float64)
            # ����� �������� ���� �����������, �����, ���������� �� �������, ��������������� �������,
            # �������� � ���, ��� ����� ������� ���� �������� ���������, ��������� � ���, ��� ��������
            # ������ �������� ��� �������
            cv2.grabCut(img2,mask,rect,bgdmodel,fgdmodel,1,cv2.GC_INIT_WITH_RECT)
            rect_or_mask = 1
        elif rect_or_mask == 1:         # ����������� �� �����
            bgdmodel = np.zeros((1,65),np.float64)
            fgdmodel = np.zeros((1,65),np.float64)
            cv2.grabCut(img2,mask,rect,bgdmodel,fgdmodel,1,cv2.GC_INIT_WITH_MASK)

    # ����������� �����. ������� 1 � 3 � 255, ��������� � 0
    mask2 = np.where((mask==1) + (mask==3),255,0).astype('uint8')
    # ��������� � �����, ��� ����� ��� ����� ������ ������.
    output = cv2.bitwise_and(img2,img2,mask=mask2)

cv2.destroyAllWindows()
