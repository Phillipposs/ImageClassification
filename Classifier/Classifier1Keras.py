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
import json
# Process Model

lista = sys.argv

model = VGG16(weights="imagenet")
filename = lista[1]
im=Image.open(filename)
image = load_img(filename, target_size=(224, 224))
image = img_to_array(image)
image = image.reshape((1, image.shape[0], image.shape[1], image.shape[2]))
image = preprocess_input(image)
pred = model.predict(image)
P = decode_predictions(pred)
# probabilities to our terminal
niz = []
for (i, (imagenetID, label, prob)) in enumerate(P[0]):
    niz.append(("{}. {}: {:.2f}%".format(i + 1, label, prob * 100))) 


print(json.dumps(niz))
#print('Predicted:', decode_predictions(pred, top=3)[0])

