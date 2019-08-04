using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Weapons
{
    internal sealed class IncreaseStrength : IWeapon
    {
        private readonly string mTextureIdentifier;
        private readonly int mRange;
        private readonly int mDamageIncreaseValue;
        private readonly float mAttackRateIncreaseValue;
        private List<ICharacter> mCharacterInCircle;
        private BattleModeState.CharactersInRadius mCharactersInRadius;
        private bool mInitialize;

        /// <summary>
        /// Weapon that increases damage and decreases attack rate of characters inside a circle
        /// </summary>
        /// <param name="textureIdentifier"></param>
        /// <param name="charactersInRadius"></param>
        /// <param name="player"></param>
        /// <param name="range"></param>
        /// <param name="damageIncrease"></param>
        /// <param name="attackRateDecrease"></param>
        public IncreaseStrength(string textureIdentifier, BattleModeState.CharactersInRadius charactersInRadius, 
            bool player, int range = 100, int damageIncrease = 20, float attackRateDecrease = 1)
        {
            mTextureIdentifier = textureIdentifier;
            Player = player;
            mCharactersInRadius = charactersInRadius;
            mRange = range;
            mDamageIncreaseValue = damageIncrease;
            mAttackRateIncreaseValue = attackRateDecrease;
            mCharacterInCircle = null;
            SecondsSinceShot = 0;
            AttackRate = 1;
            mInitialize = true;

        }

        public int MaxShieldDamage { get; set; }
        public float SecondsSinceShot { get; set; }
        public bool Player { get; set; }
        public int Damage { get; set; }
        public float AttackRate { get; set; }


        /// <summary>
        /// Dictionary for updating damage and attack rate for all characters.
        /// If a character is in the Circle the first value becomes true,
        /// if the characters damage and attack rate has already been increased the second value becomes true
        /// </summary>
        /// <param name="position"></param>
        private void InitializeList(Vector2 position)
        {
            mCharacterInCircle = mCharactersInRadius.Invoke(position, mRange, Player);
            foreach (var character in mCharacterInCircle)
            {
                Increase(character);
            }
            mInitialize = false;
        }

        private void Increase(ICharacter c)
        {
            if (c.Weapon == null) return;
            c.Weapon.AttackRate += mAttackRateIncreaseValue;
            c.Weapon.Damage += mDamageIncreaseValue;
        }

        private void Decrease(ICharacter c)
        {
            if (c.Weapon == null || mCharacterInCircle[0].Player != Player) return;
            c.Weapon.AttackRate -= mAttackRateIncreaseValue;
            c.Weapon.Damage -= mDamageIncreaseValue;
        }

        /// <summary>
        /// Increase Damage and decrease attack rate for all characters inside the circle of the character who is using this weapon
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="position"></param>
        public void UseWeapon(GameTime gameTime, Vector2 position)
        {
            if (mInitialize)
            {
                InitializeList(position);
            }
            var newCharactersInCircle = mCharactersInRadius.Invoke(position, mRange, Player);
            var enteringCharacters = newCharactersInCircle.Except(mCharacterInCircle);
            var leavingCharacters = mCharacterInCircle.Except(newCharactersInCircle);
            foreach (var leavingCharacter in leavingCharacters)
            {
                Decrease(leavingCharacter);
            }

            foreach (var enteringCharacter in enteringCharacters)
            {
                Increase(enteringCharacter);
            }

            mCharacterInCircle = newCharactersInCircle;
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager, Vector2 position)
        {
            // Position of circle centered around Character
            var rectangle = new Rectangle((int) position.X - mRange,
                (int) position.Y - mRange, mRange*2, mRange*2);
            sb.Draw(textureManager.GetTexture(mTextureIdentifier), rectangle, Color.AliceBlue);
        }
    }
}