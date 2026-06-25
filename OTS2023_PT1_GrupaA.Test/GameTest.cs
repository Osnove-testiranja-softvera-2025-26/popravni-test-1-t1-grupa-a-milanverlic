using System;
using NUnit.Framework;
using OTS2026_PT1_GrupaA.Exceptions;
using OTS2026_PT1_GrupaA.Models;

namespace OTS2026_PT1_GrupaA.Test
{
    [TestFixture]
    internal class GameTest
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game(new Position(0, 5), new Position(1, 5));
        }

        // ECP: Nevalidna klasa ekvivalencije (igrač se nalazi u Invalid zoni umesto u Land zoni)
        [Test]
        public void Game_InvalidPlayerPosition_ThrowsException()
        {
            Exception ex = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() => new Game(new Position(0, 0), new Position(1, 5))));
            Assert.That(ex.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
        }

        // ECP: Nevalidna klasa ekvivalencije (čamac se nalazi u Pond zoni umesto u Land zoni)
        [Test]
        public void Game_InvalidBoatPosition_ThrowsException()
        {
            Exception ex = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() => new Game(new Position(0, 5), new Position(20, 0))));
            Assert.That(ex.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
        }

        // ECP: Validne klase ekvivalencije za sve moguće pravce kretanja (Up, Down, Left, Right)
        [TestCase(Move.Up, 1, 6, 1, 5)]
        [TestCase(Move.Down, 1, 6, 1, 7)]
        [TestCase(Move.Left, 1, 6, 0, 6)]
        [TestCase(Move.Right, 1, 6, 2, 6)]
        public void MovePlayer_ValidInput_PlayerMoves(Move move, int x, int y, int ex, int ey)
        {
            game.Player.Position = new Position(x, y);
            game.MovePlayer(move);
            Assert.That(game.Player.Position, Is.EqualTo(new Position(ex, ey)));
        }

        // ECP: Validna klasa ekvivalencije (pozicija se nalazi unutar dozvoljene Land zone)
        [Test]
        public void ValidatePosition_ValidPosition_ReturnsTrue()
        {
            bool result = game.ValidatePosition(new Position(5, 10));
            Assert.That(result, Is.True);
        }

        // BVA: Analiza graničnih vrednosti (koordinate van granica matrice, X manje od 0)
        [Test]
        public void ValidatePosition_PositionOutsideMap_ReturnsFalse()
        {
            bool result = game.ValidatePosition(new Position(-1, 0));
            Assert.That(result, Is.False);
        }

        // Decision Table: Uslov 1 (Pond zona) ispunjen, Uslov 2 (HasBoat == false) nije ispunjen -> False
        [Test]
        public void ValidatePosition_PondZoneWithoutBoat_ReturnsFalse()
        {
            game.Player.HasBoat = false;
            bool result = game.ValidatePosition(new Position(20, 0));
            Assert.That(result, Is.False);
        }

        // Decision Table: Uslov 1 (Pond zona) ispunjen, Uslov 2 (HasBoat == true) ispunjen -> True
        [Test]
        public void ValidatePosition_PondZoneWithBoat_ReturnsTrue()
        {
            game.Player.HasBoat = true;
            bool result = game.ValidatePosition(new Position(20, 0));
            Assert.That(result, Is.True);
        }

        // ECP: Nevalidna klasa ekvivalencije (pozicija pogađa Invalid zonu u kojoj je zabranjeno kretanje)
        [Test]
        public void ValidatePosition_InvalidZone_ReturnsFalse()
        {
            bool result = game.ValidatePosition(new Position(0, 0));
            Assert.That(result, Is.False);
        }

        // ECP: Validna klasa ekvivalencije (stajanje na polje tipa Bait)
        [Test]
        public void ResolvePlayerPosition_BaitTile_BaitIncreased()
        {
            game.Player.Position = new Position(0, 5);
            game.Player.AmountOfBait = 0;
            game.Map.Fields[0, 5].Content = FieldContent.Bait;
            game.ResolvePlayerPosition();
            Assert.That(game.Player.AmountOfBait, Is.EqualTo(1));
        }

        // ECP: Validna klasa ekvivalencije (stajanje na polje tipa Boat)
        [Test]
        public void ResolvePlayerPosition_BoatTile_HasBoatIsTrue()
        {
            game.Player.Position = new Position(0, 5);
            game.Map.Fields[0, 5].Content = FieldContent.Boat;
            game.ResolvePlayerPosition();
            Assert.That(game.Player.HasBoat, Is.True);
        }

        // Decision Table: Polje je Fish (True) AND Igrač ima mamac (True) -> Pecanje uspešno
        [Test]
        public void ResolvePlayerPosition_FishTileWithBait_FishCaught()
        {
            game.Player.Position = new Position(20, 0);
            game.Player.AmountOfBait = 1;
            game.Player.AmountOfFish = 0;
            game.Map.Fields[20, 0].Content = FieldContent.Fish;
            game.ResolvePlayerPosition();
            Assert.That(game.Player.AmountOfFish, Is.EqualTo(1));
        }

        // Decision Table: Polje je Fish (True) AND Igrač ima mamac (False) -> Pecanje neuspešno
        [Test]
        public void ResolvePlayerPosition_FishTileWithoutBait_NothingChanges()
        {
            game.Player.Position = new Position(20, 0);
            game.Player.AmountOfBait = 0;
            game.Player.AmountOfFish = 0;
            game.Map.Fields[20, 0].Content = FieldContent.Fish;
            game.ResolvePlayerPosition();
            Assert.That(game.Player.AmountOfFish, Is.EqualTo(0));
        }

        // PICT metoda
        [TestCaseSource(typeof(GameTestDataFromFile), "Get_CalculateIncome_OKInput_SuccessfulCalculation_TestData", new object[] { "data_calculate_income.txt" })]
        public void CalculateIncome_OKInput_SuccessfulCalculation(int amountOfFish, int amountOfBait, bool hasBoat, Game.Score expectedScore)
        {
            game.Player.AmountOfFish = amountOfFish;
            game.Player.AmountOfBait = amountOfBait;
            game.Player.HasBoat = hasBoat;

            Game.Score actualScore = game.CalculateIncome();

            Assert.That(actualScore, Is.EqualTo(expectedScore));
        }
    }
}