using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipSecuritySystem
{
    /// <summary>
    /// ChipService class is responsible for managing the generation, storage, and validation of color chips against a master chip.
    /// </summary>
    internal class ChipService
    {
        private readonly List<ColorChip> chips;
        private readonly ColorChip masterChip;
        private bool isMasterPanelUnlocked;
        private static readonly Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the ChipService class with an empty chip collection, a default
        /// master chip, and a locked master panel state.
        /// </summary>
        /// <remarks>The default master chip uses blue and green as the beginning and ending markers.</remarks>
        public ChipService()
        {
            this.chips = new List<ColorChip>();
            this.masterChip = new ColorChip(Color.Blue, Color.Green); // Default master chip contains the Beginning and Ending markers
            this.isMasterPanelUnlocked = false;
        }

        public void AddChip(ColorChip chip)
        {
            this.chips.Add(chip);
        }

        public string GetMasterChip()
        {
            return this.masterChip.ToString();
        }


        /// <summary>
        /// Generate a chip with random colors
        /// </summary>
        /// <returns></returns>
        public ColorChip GenerateRandomChip()
        {
            Array colors = Enum.GetValues(typeof(Color));
            Color startColor = (Color)colors.GetValue(random.Next(colors.Length));
            // Ensure that the end color is different from the start color
            Color endColor;
            do
            {
                endColor = (Color)colors.GetValue(random.Next(colors.Length));
            } while (endColor == startColor);

            return new ColorChip(startColor, endColor);
        }

        /// <summary>
        /// Creates a chip series and update the corresponding field.
        /// </summary>
        /// <param name="numberOfChips">Number of chips</param>
        public void CreateChipSeries(int numberOfChips)
        {
            for (int i = 0; i < numberOfChips; i++)
            {
                this.AddChip(this.GenerateRandomChip());
            }
        }

        /// <summary>
        /// Get the string corresponding to the Chip List.
        /// </summary>
        /// <param name="numberOfChips"></param>
        /// <returns></returns>
        public string GetChipList(int numberOfChips)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var chip in this.chips.Take(numberOfChips))
            {
                sb.AppendLine(chip.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Main method to validate if the chip series grnerated previously is valid
        /// </summary>
        public void ValidateChipSeries()
        {
            bool hasStartMatch = this.chips.Any(chip => chip.StartColor == this.masterChip.StartColor);
            bool hasEndMatch = this.chips.Any(chip => chip.EndColor == this.masterChip.EndColor);

            if (!hasStartMatch)
            {
                Console.WriteLine("...Master panel not unlocked. There is no chip that matches the master chip's start color.");
                this.isMasterPanelUnlocked = false;
                return;
            }

            if (!hasEndMatch)
            {
                Console.WriteLine("...Master panel not unlocked. There is no chip that matches the master chip's end color.");
                this.isMasterPanelUnlocked = false;
                return;
            }

            List<ColorChip> bestSequence = FindLongestValidSequence();

            // Minimum valid sequence length is 2 chips
            if (bestSequence.Count < 2)
            {
                Console.WriteLine("...Master panel not unlocked. A valid sequence must contain at least 2 chips.");
                this.isMasterPanelUnlocked = false;
                return;
            }

            this.isMasterPanelUnlocked = true;
            Console.WriteLine("...Master panel unlocked with a series of " + bestSequence.Count + " chips.");
            Console.WriteLine("*** Sequence: " + string.Join(", ", bestSequence.Select(chip => $"[{chip.StartColor}, {chip.EndColor}]")));

            if (this.chips.Count > bestSequence.Count)
            {
                var unusedChips = this.chips.Except(bestSequence).ToList();
                Console.WriteLine("...Unused chips: " + string.Join(", ", unusedChips.Select(chip => $"[{chip.StartColor}, {chip.EndColor}]")));
            }
        }

        /// <summary>
        /// Find the longest valid sequence of chips in the existing list
        /// </summary>
        /// <returns>The longest chip list found</returns>
        private List<ColorChip> FindLongestValidSequence()
        {
            var bestPathIndexes = new List<int>();
            var currentPathIndexes = new List<int>();
            var usedIndexes = new HashSet<int>();

            // Start only with chips matching master start
            for (int i = 0; i < this.chips.Count; i++)
            {
                if (this.chips[i].StartColor != this.masterChip.StartColor)
                {
                    continue;
                }

                usedIndexes.Add(i);
                currentPathIndexes.Add(i);

                DepthSearch(this.chips[i].EndColor, usedIndexes, currentPathIndexes, ref bestPathIndexes);

                currentPathIndexes.RemoveAt(currentPathIndexes.Count - 1);
                usedIndexes.Remove(i);
            }

            return bestPathIndexes.Select(index => this.chips[index]).ToList();
        }

        /// <summary>
        /// Runs as depth search to find the longest valid sequence of chips. 
        /// It recursively explores all possible paths starting from the current color, keeping track of used indexes and the current path. 
        /// If a valid path is found that ends with the master chip's end color and is longer than the best path found so far, it updates the best path.
        /// </summary>
        /// <param name="currentColor">The current color to match the start color of the next chip.</param>
        /// <param name="usedIndexes">A set of indexes of chips that have already been used in the current path.</param>
        /// <param name="currentPathIndexes">A list of indexes representing the current path of chips being explored.</param>
        /// <param name="bestPathIndexes">A reference to the list of indexes representing the best path found so far.</param>
        private void DepthSearch(
                                 Color currentColor,
                                 HashSet<int> usedIndexes,
                                 List<int> currentPathIndexes,
                                 ref List<int> bestPathIndexes)
        {
            // Valid only if last chip ends at master end color AND path has at least 2 chips
            if (currentColor == this.masterChip.EndColor &&
                currentPathIndexes.Count >= 2 &&
                currentPathIndexes.Count > bestPathIndexes.Count)
            {
                bestPathIndexes = new List<int>(currentPathIndexes);
            }

            for (int i = 0; i < this.chips.Count; i++)
            {
                if (usedIndexes.Contains(i))
                {
                    continue;
                }

                var chip = this.chips[i];
                if (chip.StartColor != currentColor)
                {
                    continue;
                }

                usedIndexes.Add(i);
                currentPathIndexes.Add(i);

                DepthSearch(chip.EndColor, usedIndexes, currentPathIndexes, ref bestPathIndexes);

                currentPathIndexes.RemoveAt(currentPathIndexes.Count - 1);
                usedIndexes.Remove(i);
            }
        }
    }
}