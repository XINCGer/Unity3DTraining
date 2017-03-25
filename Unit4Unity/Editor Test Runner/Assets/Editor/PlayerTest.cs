using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerTest{

    [Test]
    public void TestHealth()
    {
        Player player = new Player();
        player.Health = 1000.0f;
        
        //通过Assert断言来判断这个函数的返回结果是否符合预期
        player.Damage(200);
        Assert.AreEqual(800,player.Health);

        player.Recover(150);
        Assert.AreEqual(950,player.Health);
    }

    [Test]
    [ExpectedException(typeof(NegativeHealthException))]
    public void NegativeHealth()
    {
        Player player = new Player();
        player.Health = 1000;

        player.Damage(500);
        player.Damage(600);
    }

}
