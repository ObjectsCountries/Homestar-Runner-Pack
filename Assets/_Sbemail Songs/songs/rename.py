import os

notincludedsbemails=[6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 28, 29, 30, 31, 34, 35, 37, 38, 41, 42, 47, 48, 49, 51, 52, 53, 54, 56, 57, 61, 62, 64, 65, 67, 68, 72, 73, 76, 78, 81, 83, 84, 85, 88, 91, 92, 93, 94, 95, 101, 103, 105, 107, 110, 115, 117, 119, 123, 142, 149, 166, 170, 172, 181, 184, 189, 200, 201, 202]
x=1
for i in range(0,131):
    if i<10:
        os.rename('sbemailsongs-0'+str(i)+'.ogg','AUDIO_ss_'+str(x)+'.ogg')
    else:
        os.rename('sbemailsongs-'+str(i)+'.ogg','AUDIO_ss_'+str(x)+'.ogg')
    x+=1
    while(x in notincludedsbemails):
        x+=1
