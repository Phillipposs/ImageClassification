# Import Libraries
from keras.preprocessing.image import load_img
from keras.preprocessing.image import img_to_array
from keras.applications.vgg16 import preprocess_input
from keras.applications.vgg16 import decode_predictions
from keras.applications.vgg16 import VGG16
from keras import backend as K
import glob
import matplotlib.pyplot as plt
import numpy as np
import sys
import os
from pathlib import Path
from PIL import Image
import threading
import threading as thread_locker
import time
# Process Model

lista = sys.argv
model = VGG16()
dirName = lista[1]
reportsDir ="/reports" 
reportsPath = os.getcwd() + reportsDir
path = dirName+reportsDir


def makeDir():
   
    try:
        os.mkdir(path)
    except OSError:
        print ("Creation of the directory %s failed" % path)
    else:
        print ("Successfully created the directory %s " % path)
        
def createTxtFile(fileName,text):
     file = open(fileName+".txt","w") 
     file.write(text) 
     file.close() 
     
def classifyFilesInFolder(): 
    
    makeDir()    
    while(1>0):
        listOfFiles = os.listdir(dirName)
        if len(listOfFiles) != 0:
            for filename in listOfFiles:
                fileInDir = dirName+"/"+filename
                if  not os.path.isfile(fileInDir):
                    continue
                reportDirFile = dirName+reportsDir+"/"+filename
                image = load_img(fileInDir, target_size=(224, 224))
                image = img_to_array(image)
                image = image.reshape((1, image.shape[0], image.shape[1], image.shape[2]))
                image = preprocess_input(image)
                pred = model.predict(image, batch_size = 16)
                content = decode_predictions(pred, top=3)[0]
                createTxtFile(reportDirFile,str(content))
                os.remove(fileInDir)
                print('Predicted:',content)
        time.sleep(1)
model._make_predict_function()
thread1 = threading.Thread(target = classifyFilesInFolder,args = ())
thread1.start()
