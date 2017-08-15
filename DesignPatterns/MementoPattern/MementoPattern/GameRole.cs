using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern
{
    /// <summary>
    /// 游戏角色信息
    /// </summary>
    class GameRole
    {
        /// <summary>
        /// 角色等级
        /// </summary>
        private int Level;
        /// <summary>
        /// 地图坐标
        /// </summary>
        private int Coordinate;
        /// <summary>
        /// 章节进度
        /// </summary>
        private int Chapter;

        private int HP;
        private int MP;

        public GameState SaveState()
        {
            return new GameState(Level,Coordinate,Chapter,HP,MP);
        }

        public void Recover(GameState gameState)
        {
            this.Level = gameState.Level;
            this.Coordinate = gameState.Coordinate;
            this.Chapter = gameState.Chapter;
            this.HP = gameState.Hp;
            this.MP = gameState.Mp;
        }

        public void ShowState()
        {
            Console.WriteLine("进度信息如下：");
            Console.WriteLine("级别："+this.Level);
            Console.WriteLine("坐标："+this.Coordinate);
            Console.WriteLine("生命："+this.HP);
            Console.WriteLine("魔力："+this.MP);
            Console.WriteLine("进度："+this.Chapter);
        }

        public void Init()
        {
            this.Level = 100;
            this.Chapter = 10;
            this.Coordinate = 999;
            this.HP = 8000;
            this.MP = 999;
        }

        public void DoTask()
        {
            this.Level = 101;
            this.Chapter = 11;
            this.Coordinate = 666;
            this.HP = 0;
            this.MP = 0;
        }
    }
}
