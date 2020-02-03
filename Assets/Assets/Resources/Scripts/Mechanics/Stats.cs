using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RedKite
{
    public class Stats
    {
        public Ability Strength;
        public Ability Intelligence;
        public Ability Wisdom;
        public Ability Dexterity;
        public Ability Constitution;
        public Ability Charisma;


        public Stats(JobClass jobClass)
        {
            System.Random rnd = new Random();

            int[] rolls = new int[6];

            rolls[0] = 4 + rnd.Next(1,7) + rnd.Next(1,7);
            rolls[1] = 4 + rnd.Next(1, 7) + rnd.Next(1, 7);
            rolls[2] = 4 + rnd.Next(1, 7) + rnd.Next(1, 7);
            rolls[3] = 4 + rnd.Next(1, 7) + rnd.Next(1, 7);
            rolls[4] = 4 + rnd.Next(1, 7) + rnd.Next(1, 7);
            rolls[5] = 4 + rnd.Next(1, 7) + rnd.Next(1, 7);

            Strength = new Ability();
            Constitution = new Ability();
            Dexterity = new Ability();
            Intelligence = new Ability();
            Wisdom = new Ability();
            Charisma = new Ability();

            DistributeRolls(rolls, jobClass);
        }

        public void LevelUp(JobClass jobClass)
        {
            System.Random rnd = new Random();

            int[] rolls = new int[6];

            //this needs to be adjusted to not always give highest to major attribute.
            //they need to be most likely to level up but shouldn't always.
            rolls[0] = rnd.Next(0, 2);
            rolls[1] = rnd.Next(0, 2);
            rolls[2] = rnd.Next(0, 2);
            rolls[3] = rnd.Next(0, 2);
            rolls[4] = rnd.Next(0, 2);
            rolls[5] = rnd.Next(0, 2);

            DistributeRolls(rolls, jobClass);
        }

        void DistributeRolls(int[] rolls, JobClass jobClass)
        {
            Array.Sort(rolls);
            Array.Reverse(rolls);

            List<int> remainder = Enumerable.Range(2, 4).ToList();
            Utility.Shuffle<int>(remainder);

            if (jobClass == JobClass.Fighter)
            {
                Strength.Increase(rolls[0]);
                Dexterity.Increase(rolls[1]);

                Intelligence.Increase(rolls[remainder[0]]);
                Wisdom.Increase(rolls[remainder[1]]);
                Constitution.Increase(rolls[remainder[2]]);
                Charisma.Increase(rolls[remainder[3]]);
            }
            else if (jobClass == JobClass.Bandit)
            {
                Constitution.Increase(rolls[0]);
                Strength.Increase(rolls[1]);

                Intelligence.Increase(rolls[remainder[0]]);
                Wisdom.Increase(rolls[remainder[1]]);
                Dexterity.Increase(rolls[remainder[2]]);
                Charisma.Increase(rolls[remainder[3]]);
            }
            else if (jobClass == JobClass.Ranger)
            {
                Dexterity.Increase(rolls[0]);
                Charisma.Increase(rolls[1]);

                Constitution.Increase(rolls[remainder[0]]);
                Wisdom.Increase(rolls[remainder[1]]);
                Strength.Increase(rolls[remainder[2]]);
                Intelligence.Increase(rolls[remainder[3]]);
            }
            else if (jobClass == JobClass.Cleric)
            {
                Wisdom.Increase(rolls[0]);
                Constitution.Increase(rolls[1]);

                Charisma.Increase(rolls[remainder[0]]);
                Dexterity.Increase(rolls[remainder[1]]);
                Strength.Increase(rolls[remainder[2]]);
                Intelligence.Increase(rolls[remainder[3]]);
            }
            else if (jobClass == JobClass.Mage)
            {
                Intelligence.Increase(rolls[0]);
                Wisdom.Increase(rolls[1]);

                Constitution.Increase(rolls[remainder[0]]);
                Dexterity.Increase(rolls[remainder[1]]);
                Strength.Increase(rolls[remainder[2]]);
                Charisma.Increase(rolls[remainder[3]]);
            }
            else
            {
                Charisma.Increase(rolls[0]);
                Intelligence.Increase(rolls[1]);

                Wisdom.Increase(rolls[remainder[0]]);
                Dexterity.Increase(rolls[remainder[1]]);
                Strength.Increase(rolls[remainder[2]]);
                Constitution.Increase(rolls[remainder[3]]);
            }

        }
    }
}
