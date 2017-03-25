using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


class PlayerTestWrong
{
    [Test]
    public void TestHealthWrong()
    {
        Player player = new Player();
        player.Health = 1000.0f;
        
        player.DamageWrong(200);
        Assert.AreEqual(800,player.Health);

        player.DamageWrong(150);
        Assert.AreEqual(950,player.Health);
    }

    [Test]
    [ExpectedException(typeof (NegativeHealthException))]
    public void NegativeHealthNoException()
    {
        Player player = new Player();
        player.Health = 1000;

        player.DamageNoException(500);
        player.DamageNoException(600);
    }
}

