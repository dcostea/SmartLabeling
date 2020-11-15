using System;
using System.IO;
using System.Linq;
using Microsoft.ML;

namespace SmartLabeling.ML.DeepLearning
{
    //public class ImageNetDataProbability : ImageNetData
    //{
    //    public float Probability { get; set; }

    //    public string PredictedLabel { get; set; }
    //    public RectangleF Rectangle { get; set; }
    //}

    //////public class ImageNetPrediction
    //////{
    //////    public float[] Score;
    //////    public string PredictedLabelValue;
    //////}

    //public struct InceptionSettings
    //{
    //    // input tensor name
    //    public const string inputTensorName = "input";

    //    // output tensor name
    //    public const string outputTensorName = "softmax2";
    //}

    //public class ImageNetPredictions
    //{
    //    [ColumnName(InceptionSettings.outputTensorName)]
    //    public float[] PredictedLabels;
    //}

    public class Inception
    {
        private const string OUTPUT_LAYER = "softmax2_pre_activation";
        private const string INPUT_LAYER = "input";

        private static readonly string LabelToKey = nameof(LabelToKey);
        private static readonly string ImageReal = nameof(ImageReal);
        private static readonly string PredictedLabelValue = nameof(PredictedLabelValue);
        private static readonly string PredictedLabel = nameof(PredictedLabel);

        private static readonly MLContext mlContext;

        public static PredictionEngine<ImageNetData, ImageNetPrediction> Model { get; set; }

        static Inception()
        {
            mlContext = new MLContext();
        }

        //public static string[] ReadLabels(string labelsLocation)
        //{
        //    return File.ReadAllLines(labelsLocation);
        //}

        //public static (string, float) GetBestLabel(string[] labels, float[] probs)
        //{
        //    var max = probs.Max();
        //    var index = probs.AsSpan().IndexOf(max);
        //    return (labels[index], max);
        //}

        //public static (string, float) GetSecondLabel(string[] labels, float[] probs)
        //{
        //    var max = probs.Max();
        //    var second = probs.Where(p => p != max).Max();
        //    var index = probs.AsSpan().IndexOf(second);
        //    return (labels[index], second);
        //}

        //public static (string, float) GetThirdLabel(string[] labels, float[] probs)
        //{
        //    var max = probs.Max();
        //    var second = probs.Where(p => p != max).Max();
        //    var third = probs.Where(p => p != max && p != second).Max();
        //    var index = probs.AsSpan().IndexOf(third);
        //    return (labels[index], third);
        //}

        public static PredictionEngine<ImageNetData, ImageNetPrediction> LoadModel(string tsv, string imagesFolder, string inceptionModel, string modelLocation)
        {
            var model = mlContext.Model.Load(modelLocation, out var modelSchema);
            var predictor = mlContext.Model.CreatePredictionEngine<ImageNetData, ImageNetPrediction>(model);

            return predictor;
        }

        public static PredictionEngine<ImageNetData, ImageNetPrediction> LoadAndScoreModel(string tsv, string imagesFolder, string inceptionModel, string modelLocation)
        {
            var data = mlContext.Data.LoadFromTextFile<ImageNetData>(path: tsv, hasHeader: false);

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: LabelToKey,
                    inputColumnName: nameof(ImageNetData.Label))
                .Append(mlContext.Transforms.LoadImages(
                    outputColumnName: INPUT_LAYER,
                    imageFolder: imagesFolder,
                    inputColumnName: nameof(ImageNetData.ImagePath)))
                .Append(mlContext.Transforms.ResizeImages(
                    outputColumnName: INPUT_LAYER,
                    imageWidth: ImageNetSettings.imageWidth,
                    imageHeight: ImageNetSettings.imageHeight,
                    inputColumnName: INPUT_LAYER))
                .Append(mlContext.Transforms.ExtractPixels(
                    outputColumnName: INPUT_LAYER,
                    interleavePixelColors: ImageNetSettings.channelsLast,
                    offsetImage: ImageNetSettings.mean))
                .Append(mlContext.Model.LoadTensorFlowModel(inceptionModel).ScoreTensorFlowModel(
                    inputColumnNames: new[] { INPUT_LAYER },
                    outputColumnNames: new[] { OUTPUT_LAYER },
                    addBatchDimensionInput: true))
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(
                    labelColumnName: LabelToKey,
                    featureColumnName: OUTPUT_LAYER))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(PredictedLabelValue, PredictedLabel))
                .AppendCacheCheckpoint(mlContext);

            ITransformer model = pipeline.Fit(data);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageNetData, ImageNetPrediction>(model);
            var trainData = model.Transform(data);
            mlContext.Model.Save(model, trainData.Schema, modelLocation);

            return predictionEngine;
        }
    }
}
