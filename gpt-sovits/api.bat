@echo off
chcp 65001
runtime\python.exe api.py -cp "。" -sm "c" -d "cuda" -dr "C:\project\voices\花火\参考音频\说话-可聪明的人从一开始就不会入局。你瞧，我是不是更聪明一点？.wav" -dt "说话-可聪明的人从一开始就不会入局。你瞧，我是不是更聪明一点？" -dl "zh"
pause
