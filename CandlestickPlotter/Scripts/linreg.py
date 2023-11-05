import sys
import numpy as np
import pandas as pd
from sklearn.linear_model import LinearRegression

length = int(sys.argv[1])
filename = "C:\\Users\\joseh\\source\\repos\\CandleStickPlotter\\CandleStickPlotter\\Scripts\\temp.csv"
y = np.genfromtxt(filename, delimiter=',').reshape(-1, 1)
X = np.arange(start=1, stop=length+1).reshape(-1,1)
n_elements = len(y) - length + 1
r2_array = np.empty(shape=(n_elements,))
coef_array = np.empty(shape=(n_elements,))
intercept_array = np.empty(shape=(n_elements,))
predict_array = np.empty(shape=(n_elements,))
for i in range(n_elements):
    reg = LinearRegression().fit(X, y[i:i + length])
    r2_array[i] = reg.score(X, y[i:i + length])
    coef_array[i] = reg.coef_[0][0]
    intercept_array[i] = reg.intercept_[0]
    predict_array[i] = reg.predict(np.array(length).reshape(-1,1))[0][0]

df = pd.DataFrame({'Slope':coef_array, 'Intercept':intercept_array, 'Predicted':predict_array, 'R2':r2_array})
df.to_csv(filename, index=False)