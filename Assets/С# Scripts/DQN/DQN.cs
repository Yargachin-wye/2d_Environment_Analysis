
using System.Numerics;
public class DQN
{
    private readonly int inputSize;
    private readonly int outputSize;
    private readonly int hiddenSize;
    private readonly double learningRate;

    private Matrix<double> weightsInputHidden;
    private Matrix<double> weightsHiddenOutput;

    public DQN(int inputSize, int outputSize, int hiddenSize, double learningRate)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.hiddenSize = hiddenSize;
        this.learningRate = learningRate;

        // Инициализация весов сети случайными значениями
        weightsInputHidden = Matrix<double>.Build.Random(hiddenSize, inputSize);
        weightsHiddenOutput = Matrix<double>.Build.Random(outputSize, hiddenSize);
    }

    // Метод для предсказания значений Q-функции
    public Vector<double> Predict(Vector<double> input)
    {
        // Прямое распространение
        var hiddenLayerOutput = Sigmoid(weightsInputHidden * input);
        var output = Sigmoid(weightsHiddenOutput * hiddenLayerOutput);
        return output;
    }

    // Метод для обновления весов сети на основе обучающего образца
    public void UpdateWeights(Vector<double> input, Vector<double> target)
    {
        // Прямое распространение
        var hiddenLayerOutput = Sigmoid(weightsInputHidden * input);
        var output = Sigmoid(weightsHiddenOutput * hiddenLayerOutput);

        // Вычисление ошибки
        var outputErrors = target - output;

        // Обратное распространение ошибки
        var hiddenErrors = weightsHiddenOutput.TransposeThisAndMultiply(outputErrors)
    .PointwiseMultiply(hiddenLayerOutput).PointwiseMultiply(1 - hiddenLayerOutput);

        // Обновление весов сети
        weightsHiddenOutput += learningRate * outputErrors.ToColumnMatrix() * hiddenLayerOutput.ToRowMatrix();
        weightsInputHidden += learningRate * hiddenErrors.ToColumnMatrix() * input.ToRowMatrix();
    }

    // Функция активации (сигмоид)
    private Vector<double> Sigmoid(Vector<double> vector)
    {
        return vector.Map(x => 1 / (1 + System.Math.Exp(-x)));
    }
}
