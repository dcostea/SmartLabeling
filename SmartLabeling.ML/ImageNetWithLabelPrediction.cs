namespace SmartLabeling.ML
{
    public class ImageNetWithLabelPrediction : ImageNetPrediction
    {
        public ImageNetWithLabelPrediction(ImageNetPrediction pred, string label)
        {
            Label = label;
            Score = pred.Score;
            PredictedLabelValue = pred.PredictedLabelValue;
        }

        public string Label;
    }
}
