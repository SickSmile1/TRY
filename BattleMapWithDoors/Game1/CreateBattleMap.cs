using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;

namespace Game1
{
    public class CreateBattleMap : Scene
    {
        public override void initialize()
        {
            addRenderer(new DefaultRenderer());

            // Load the Tiled map
            var tiledMap = content.Load<TiledMap>("BattleMap1");

            // Display the Tiled map with a TiledMapComponent
            var tiledEntity = createEntity("tiled-map");

            // Render our ground-layer under the player.
            var tiledMapGroundComp = tiledEntity.addComponent(new TiledMapComponent(tiledMap));
            tiledMapGroundComp.setLayersToRender("Boden");
            tiledMapGroundComp.renderLayer = 10;

            // Render our above-details layer after the player so the player is occluded by them when he walks behind them
            var tiledMapWallComp = tiledEntity.addComponent(new TiledMapComponent(tiledMap));
            tiledMapWallComp.setLayerToRender("Wände");
            tiledMapWallComp.renderLayer = -1;

            // Create and render our player
            var player = createEntity("player");

            // Set spawn point for the player
            var objectsLayer = tiledMap.getObjectGroup("Spawn");
            var spawn = objectsLayer.objectWithName("spawn");
            player.transform.setPosition(spawn.x, spawn.y);

            player.addComponent(new PrototypeSprite(16, 32)).setColor(Color.Red);
            player.addComponent(new TiledMapMover(tiledMap.getLayer<TiledTileLayer>("KollidierendeWände")));
            player.addComponent(new BoxCollider(-8, -16, 16, 32));

            player.addComponent<PlayerController>();

        }
    }
}
