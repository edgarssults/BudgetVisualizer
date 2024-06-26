﻿// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Ed.BudgetVisualizer.Logic
{
    public partial class BudgetVisualizer
    {
        /// <summary>
        /// model input class for BudgetVisualizer.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(0)]
            [ColumnName(@"Description")]
            public string Description { get; set; }

            [LoadColumn(1)]
            [ColumnName(@"Origin")]
            public string Origin { get; set; }

            [LoadColumn(3)]
            [ColumnName(@"IsDebit")]
            public bool IsDebit { get; set; }

            [LoadColumn(4)]
            [ColumnName(@"IsCredit")]
            public bool IsCredit { get; set; }

            [LoadColumn(5)]
            [ColumnName(@"CategoryId")]
            public float CategoryId { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for BudgetVisualizer.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"Description")]
            public float[] Description { get; set; }

            [ColumnName(@"Origin")]
            public float[] Origin { get; set; }

            [ColumnName(@"IsDebit")]
            public float[] IsDebit { get; set; }

            [ColumnName(@"IsCredit")]
            public float[] IsCredit { get; set; }

            [ColumnName(@"CategoryId")]
            public uint CategoryId { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"PredictedLabel")]
            public float PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float[] Score { get; set; }

        }

        #endregion

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var assembly = typeof(BudgetVisualizer).Assembly;
            var modelStream = assembly.GetManifestResourceStream($"Ed.BudgetVisualizer.Logic.BudgetVisualizer.mlnet");
            var mlContext = new MLContext();
            var mlModel = mlContext.Model.Load(modelStream, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        /// <summary>
        /// Use this method to predict scores for all possible labels.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static IOrderedEnumerable<KeyValuePair<string, float>> PredictAllLabels(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            var result = predEngine.Predict(input);
            return GetSortedScoresWithLabels(result);
        }

        /// <summary>
        /// Map the unlabeled result score array to the predicted label names.
        /// </summary>
        /// <param name="result">Prediction to get the labeled scores from.</param>
        /// <returns>Ordered list of label and score.</returns>
        /// <exception cref="Exception"></exception>
        public static IOrderedEnumerable<KeyValuePair<string, float>> GetSortedScoresWithLabels(ModelOutput result)
        {
            var unlabeledScores = result.Score;
            var labelNames = GetLabels(result);

            Dictionary<string, float> labledScores = new Dictionary<string, float>();
            for (int i = 0; i < labelNames.Count(); i++)
            {
                // Map the names to the predicted result score array
                var labelName = labelNames.ElementAt(i);
                labledScores.Add(labelName.ToString(), unlabeledScores[i]);
            }

            return labledScores.OrderByDescending(c => c.Value);
        }

        /// <summary>
        /// Get the ordered label names.
        /// </summary>
        /// <param name="result">Predicted result to get the labels from.</param>
        /// <returns>List of labels.</returns>
        /// <exception cref="Exception"></exception>
        private static IEnumerable<string> GetLabels(ModelOutput result)
        {
            var schema = PredictEngine.Value.OutputSchema;

            var labelColumn = schema.GetColumnOrNull("CategoryId");
            if (labelColumn == null)
            {
                throw new Exception("CategoryId column not found. Make sure the name searched for matches the name in the schema.");
            }

            // Key values contains an ordered array of the possible labels. This allows us to map the results to the correct label value.
            var keyNames = new VBuffer<float>();
            labelColumn.Value.GetKeyValues(ref keyNames);
            return keyNames.DenseValues().Select(x => x.ToString());
        }

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }
    }
}
