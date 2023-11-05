import pandas as pd

df = pd.read_csv("C:\\Users\\joseh\\OneDrive\Documents\\CSV\\AAPL.csv")
rollingMin = df['Close'].rolling(window=20).min()
rollingMax = df['Close'].rolling(window=20).max()
for i in range(len(rollingMin)):
    print(rollingMin[i], end=' ')
    print(rollingMax[i])