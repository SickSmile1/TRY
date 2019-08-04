using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Weapons
{
    internal interface IWeapon
    {
        //Use the Weapon on targets specified by the weapon
        void UseWeapon(GameTime gameTime, Vector2 position);
        void Draw(SpriteBatch sb, TextureManager textureManager, Vector2 position);
        bool Player { get; set; }
        int Damage { get; set; }
        float AttackRate { get; set; }
        int MaxShieldDamage { get; set; }
        float SecondsSinceShot { get; set; }
    }
}
