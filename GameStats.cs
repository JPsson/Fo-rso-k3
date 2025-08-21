using System;
using System.Collections.Generic;
using System.Linq;

namespace ShutTheBoxAdvanced2
{
    // 1: TYPE PARAMETER CONSTRAINTS
    // 2: By using constraints, we ensure that the type parameter T is a numeric type
    // 3: This allows us to perform mathematical operations on T, 
    //    such as calculating the average of a collection of T values
    //    and ensures that the type T is compatible with the operations we want to perform.
    //    This is good for statistics as it allows us to use the same methods for different types of statistics.
    
    interface IStatisticCollector<T> where T : struct, IComparable, IComparable<T>, IConvertible {
        void Collect(T value); // collect a game statistic.
        void Reset();          // reset the collector.
        T GetResult();         // retrieve the collected statistic result.
    }
    interface ITimeStatisticCollector : IStatisticCollector<DateTime> {
        TimeSpan GetAvgRollTime();
    }
    interface IGameStatsHandler {
        Action<double, DateTime> OnRollEvent { get; set; }
        Action OnNewGameEvent { get; set; }
        Action OnGameEndEvent { get; set; }
    }
   
    public class GameStatsHandler: IGameStatsHandler{
        public Action<double, DateTime> OnRollEvent { get; set; }
        public Action OnNewGameEvent { get; set; }
        public Action OnGameEndEvent { get; set; }

        private readonly DoubleStatisticCollector rollStatisticCollector = new DoubleStatisticCollector();
        private readonly TimeStatisticCollector timeStatisticCollector = new TimeStatisticCollector();
        private readonly List<Achievement<double>> rollAchievements = new List<Achievement<double>>();
        private readonly List<Achievement<DateTime>> timeAchievements = new List<Achievement<DateTime>>();

        public GameStatsHandler(){
            // Initialize achievements with conditions, using Func<T, bool> for flexible conditions.
            rollAchievements.Add(new Achievement<double>("Weighted Dice", avgScore => avgScore > 5));
            rollAchievements.Add(new Achievement<double>("Don't get into gambling.", avgScore => avgScore < 3));
            timeAchievements.Add(new Achievement<DateTime>("Night Owl", time => time.TimeOfDay > TimeSpan.FromHours(20) || time.TimeOfDay < TimeSpan.FromHours(6)));
            timeAchievements.Add(new Achievement<DateTime>("Shouldn't You be working??", time => time.TimeOfDay > TimeSpan.FromHours(10) && time.TimeOfDay < TimeSpan.FromHours(16)));
            timeAchievements.Add(new Achievement<DateTime>("3l173", time => time.Hour == 13 && time.Minute == 37));

            OnRollEvent += (rollValue, rollTime) => {
                timeStatisticCollector.Collect(rollTime);
                rollStatisticCollector.Collect(rollValue);
            };
            OnNewGameEvent += () => {
                timeStatisticCollector.Reset();
                rollStatisticCollector.Reset();
                timeStatisticCollector.Collect(DateTime.Now);
            };
            OnGameEndEvent += () => {
                Console.WriteLine(GetEndScreen());
            };
        }
        string GetEndScreen(){
            bool achievementsUnlocked = false;
            string endScreen = $"";
            endScreen += $"\nSTATISTICS:";
            endScreen += $"\nAverage Roll: {rollStatisticCollector.GetResult()}";
            endScreen += $"\nGame started: {timeStatisticCollector.GetResult()}";
            endScreen += $"\nGame ended: {DateTime.Now}";
            endScreen += $"\nAverage Roll Time in milliseconds: {timeStatisticCollector.GetAvgRollTime().Milliseconds}";
            endScreen += $"\n\nACHIEVEMENTS:";
            foreach (var achievement in rollAchievements.Where(a => a.IsAchieved(rollStatisticCollector.GetResult()))) {
                endScreen += $"\nAchievement Unlocked: {achievement.Name}";
                achievementsUnlocked = true;
            }
            foreach (var achievement in timeAchievements.Where(a => a.IsAchieved(timeStatisticCollector.GetResult()))) {
                endScreen += $"\nAchievement Unlocked: {achievement.Name}";
                achievementsUnlocked = true;
            }
            if (!achievementsUnlocked) {
                endScreen += "\nNo achievements unlocked this game.";
            }
            endScreen += $"\nThanks for playing!\n";
            return endScreen;
        }
    }

    
    // 1: INVARIANCE
    // 2: Implements IStatisticCollector specifically for doubles.
    //    It accepts only double, not any subtype or supertype, ensuring strict type usage.
    // 3: We do this to ensure that the type T is compatible with the operations we want to perform.
    //    For calulating averages doubles are (almost) the only type that makes sense.
    //    Hence we want to restrict the type to double.
    //    Having strict type usage can help us implement different behavior for different types.
    //    For example, we could implement a different class for calculating the average for integers.
    class DoubleStatisticCollector : IStatisticCollector<double> {
        private List<double> values = new List<double>();
        public void Collect(double value) => values.Add(value);
        public void Reset() => values.Clear();
        // Returns the average of collected values as a double.
        public double GetResult() => values.Any() ? (double)values.Average()/2 : 0;
    }

    class TimeStatisticCollector : ITimeStatisticCollector {
        private List<DateTime> values = new List<DateTime>();
        public void Collect(DateTime value) => values.Add(value);
        public void Reset() => values.Clear();

        public DateTime GetResult(){
            // Return a default value when the collection is empty
            if (!values.Any()){return default(DateTime);}

            var sortedValues = values.OrderBy(t => t).ToList();
            // Returns the time of the first roll.
            return sortedValues.First();
        }
        public TimeSpan GetAvgRollTime()
        {
            if (values.Count < 2){return TimeSpan.Zero;}
            var sortedValues = values.OrderBy(t => t).ToList();
            TimeSpan totalSpan = sortedValues.Last() - sortedValues.First();
            TimeSpan averageSpan = new TimeSpan(totalSpan.Ticks / (values.Count - 1));
            return averageSpan;
        }
    }

    // 1: BUILT IN DELEGATES 
    // 2: Here we use the built-in delegate (Func<T, bool>) to define generic conditions for achievements
    //    We use this a parameter for the constructor of the Achievement class.
    //    The delegate is used to check if collected statistics meet the conditions for achievements.
    // 3: We use this to be able to define the conditions for achievements in a flexible way
    //    Achievements can use any condition that takes a T and returns a bool.
    //    This allows us to define achievements for any type of statistic.
    //    In the future we could add other types of achievements other than just statistics.
    class Achievement<T> where T : struct {
        public string Name { get; }
        private readonly Func<T, bool> condition;

        public Achievement(string name, Func<T, bool> condition) {
            Name = name;
            this.condition = condition;
        }
        public bool IsAchieved(T statisticValue) => condition(statisticValue);
    }
}