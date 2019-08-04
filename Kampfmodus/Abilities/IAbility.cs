using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Abilities
{
    internal interface IAbility
    {
        string Id { get; }
        int Damage { get; set; }
        float Duration { get; set; }
        float SecondsPassed { get; }
        float CoolDown { get; set;}
        float Radius { get; set; }
        bool Active { get; set; }
        void UseAbility(Vector2 position);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch sb, TextureManager tex);
        void Terminate();
    }
}
