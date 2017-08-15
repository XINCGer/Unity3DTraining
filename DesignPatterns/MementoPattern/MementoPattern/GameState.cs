using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern
{
    /// <summary>
    /// 游戏备忘录类
    /// </summary>
    class GameState
    {
        /// <summary>
        /// 等级
        /// </summary>
        private int _level;

        private int _chapter;
        private int _coordinate;
        private int HP;
        private int MP;


        /// <summary>
        /// 等级
        /// </summary>
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public int Chapter
        {
            get { return _chapter; }
            set { _chapter = value; }
        }

        public int Coordinate
        {
            get { return _coordinate; }
            set { _coordinate = value; }
        }

        public int Hp
        {
            get { return HP; }
            set { HP = value; }
        }

        public int Mp
        {
            get { return MP; }
            set { MP = value; }
        }

        public GameState(int level, int coor, int chapter, int hp, int mp)
        {
            this.Level = level;
            this.Coordinate = coor;
            this.Chapter = chapter;
            this.HP = hp;
            this.MP = mp;
        }
    }
}
