
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

        // ������������� ����� ���� ���������� ����������
        weightsInputHidden = Matrix<double>.Build.Random(hiddenSize, inputSize);
        weightsHiddenOutput = Matrix<double>.Build.Random(outputSize, hiddenSize);
    }

    // ����� ��� ������������ �������� Q-�������
    public Vector<double> Predict(Vector<double> input)
    {
        // ������ ���������������
        var hiddenLayerOutput = Sigmoid(weightsInputHidden * input);
        var output = Sigmoid(weightsHiddenOutput * hiddenLayerOutput);
        return output;
    }

    // ����� ��� ���������� ����� ���� �� ������ ���������� �������
    public void UpdateWeights(Vector<double> input, Vector<double> target)
    {
        // ������ ���������������
        var hiddenLayerOutput = Sigmoid(weightsInputHidden * input);
        var output = Sigmoid(weightsHiddenOutput * hiddenLayerOutput);

        // ���������� ������
        var outputErrors = target - output;

        // �������� ��������������� ������
        var hiddenErrors = weightsHiddenOutput.TransposeThisAndMultiply(outputErrors)
    .PointwiseMultiply(hiddenLayerOutput).PointwiseMultiply(1 - hiddenLayerOutput);

        // ���������� ����� ����
        weightsHiddenOutput += learningRate * outputErrors.ToColumnMatrix() * hiddenLayerOutput.ToRowMatrix();
        weightsInputHidden += learningRate * hiddenErrors.ToColumnMatrix() * input.ToRowMatrix();
    }

    // ������� ��������� (�������)
    private Vector<double> Sigmoid(Vector<double> vector)
    {
        return vector.Map(x => 1 / (1 + System.Math.Exp(-x)));
    }
}
