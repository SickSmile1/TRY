using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    internal sealed class KiPlanner
    {
        private readonly BattleModeState mBattleModeState;
        private HashSet<ICharacter> mEnemies;
        // occupied enemies for instance fighters
        private readonly HashSet<ICharacter> mBusy;
        // unoccupied enemies with spread behavior
        private HashSet<ICharacter> mUnoccupied;

        private readonly HashSet<ICharacter> mSupervisor;

        // Action list for each kind of enemy 
        private readonly Dictionary<ICharacter, KiActionManager> mBusyManager;
        private readonly Dictionary<ICharacter, KiActionManager> mFreeManager;
        private readonly Dictionary<ICharacter, KiActionManager> mSuperManager;

        // Let Supervisor move to the following coordinates stored in this list.
        private List<Vector2> mBorder;

        // takes charge of cooldown timer
        private float mSecondsPassed;

        // pseudo state machine with 6 states
        private enum States
        {
            InitScout,
            ScoutNext,
            SendHelp,
            Guard,
            Destination,
            Observe
        }

        // current state
        private States mActiveState;

        public KiPlanner(BattleModeState bms, HashSet<ICharacter> enemies)
        {
            mEnemies = enemies;
            mBattleModeState = bms;
            mUnoccupied = mEnemies;
            mBusy = new HashSet<ICharacter>();
            mSupervisor = new HashSet<ICharacter>();
            mBusyManager = new Dictionary<ICharacter, KiActionManager>();
            mFreeManager = new Dictionary<ICharacter, KiActionManager>();
            mSuperManager = new Dictionary<ICharacter, KiActionManager>();
            SetBorder();
            InitState();
            // init state
            mActiveState = States.InitScout;
            mSecondsPassed = 20;
        }

        /// <summary>
        /// call this function whenever new enemy spawns
        /// </summary>
        public void Reinforcement(HashSet<ICharacter> enemies)
        {
            mEnemies = enemies;
            mUnoccupied = mEnemies;
            InitState();
            StartScout();
        }

        /// <summary>
        /// set corner of the map
        /// </summary>
        private void SetBorder()
        {
            mBorder = new List<Vector2>()
            {
                new Vector2(200, 340),
                new Vector2(180, 2050),
                new Vector2(3900, 2040),
                new Vector2(3900, 230)
            };
        }

        private void InitSupervisor()
        {
            foreach (var sup in mEnemies)
            {
                if (sup.Texture == "Supervisor" && !mSupervisor.Contains(sup))
                {
                    mSupervisor.Add(sup);
                }
            }
        }

        /// <summary>
        /// a state, where the enemy behavior is defined
        /// </summary>
        private void InitState()
        {
            InitSupervisor();

            foreach (var unoccupied in mUnoccupied)
            {
                if (mBusy.Contains(unoccupied) || mSupervisor.Contains(unoccupied) ||
                    mFreeManager.ContainsKey(unoccupied)) continue;
                var ap = new AttackPlayer(unoccupied);
                var sp = new Spread(unoccupied);
                var ep = new EvadePlayer(unoccupied);
                var mp = new MoveToPlayer(unoccupied);
                var kiManager = new KiActionManager(new IKiActions[] { ap, ep, mp, sp });
                mFreeManager.Add(unoccupied, kiManager);
            }
        }

        // Send Scouts to the four corners
        private void StartScout() 
        {
            var corner = 3;
            foreach (var sup in mSupervisor)
            {
                if (mSuperManager.ContainsKey(sup))
                {
                    continue;
                }
                var ap = new AttackPlayer(sup); 
                var ep = new EvadePlayer(sup);
                var mt = new MoveToTarget(sup);
                mt.SetTarget(mBorder[corner]);
                var kiManager = new KiActionManager(new IKiActions[] {ap, ep, mt});
                mSuperManager.Add(sup, kiManager);
                corner--;
                corner = corner < 0 ? 3 : corner;
            }
        }

        // If an enemy is found, send units to enemy's
        // location
        private void SendHelp(Vector2 targetPosition)
        {
            // Supervisor shouts to call for help
            if (mSecondsPassed > 5)
            {
                Game1.sSoundEffectInstance[1].Play();
            }

            // All free enemies who are able to hear the shout (within radius of 800)
            // will move to the target position.
            var maxHelper = mBattleModeState.FindCharactersInRadius(targetPosition, 800, false);
            foreach (var t in maxHelper)
            {
                if (mBusyManager.ContainsKey(t) || mSuperManager.ContainsKey(t))
                {
                    continue;
                }
                var ap = new AttackPlayer(t);
                var ep = new EvadePlayer(t);
                var mt = new MoveToTarget(t);
                var mp = new MoveToPlayer(t);
                var sp = new Spread(t);
                mt.SetTarget(targetPosition);
                var kiManager = new KiActionManager(new IKiActions[] { ap, ep, mt, mp, sp });
                mFreeManager.Remove(t);
                mBusy.Add(t);
                mBusyManager.Add(t, kiManager);
            }
            mSecondsPassed = 0;
        }

        /// <summary>
        /// Supervisor who reached their destination moves to the next corner
        /// </summary>
        private void ScoutNext()
        {
            foreach (var sup in mSupervisor)
            {
                for(var i = 0; i < mBorder.Count; i++)
                {
                    if (sup.MidPoint != mBorder[i]) continue;
                    mSuperManager.Remove(sup);
                    var ap = new AttackPlayer(sup);
                    var ep = new EvadePlayer(sup);
                    var mt = new MoveToTarget(sup);
                    mt.SetTarget(mBorder[(i + 1) % mBorder.Count]);
                    var kiManager = new KiActionManager(new IKiActions[] { ap, ep, mt });
                    mSuperManager.Add(sup, kiManager);
                }
            }
        }

        /// <summary>
        /// Check if corner has been reached
        /// </summary>
        /// <returns></returns>
        private bool DestinationReach()
        {
            foreach (var sup in mSupervisor)
            {
                foreach (var t in mBorder)
                {
                    if (sup.MidPoint == t)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// pseudo state machine
        /// </summary>
        /// <param name="players"></param>
        /// <param name="items"></param>
        /// <param name="cc"></param>
        private void KiBrain(HashSet<ICharacter> players, List<CollectableItem> items, List<CryoChamber> cc)
        {
            switch (mActiveState)
            {
                case States.InitScout:
                    StartScout();
                    mActiveState = States.Observe;
                    break;
                case States.ScoutNext:
                    break;
                case States.Destination:
                    ScoutNext();
                    mActiveState = States.Observe;
                    break;
                case States.Guard:
                    const int itemsCd = 10;
                    if (mSecondsPassed > itemsCd)
                    {
                        SendHelp(ObjectInSight(items, cc));
                    }
                    mActiveState = States.Observe;
                    break;
                case States.SendHelp:
                    const int cooldown = 5;
                    if (mSecondsPassed > cooldown)
                    {
                        SendHelp(EnemyInRange(players));
                    }
                    mActiveState = States.Observe;
                    break;
                case States.Observe:
                    if (EnemyInRange(players) != Vector2.Zero)
                    {
                        mActiveState = States.SendHelp;
                    }

                    else if (ObjectInSight(items, cc) != Vector2.Zero)
                    {
                        mActiveState = States.Guard;
                    }

                    if (DestinationReach())
                    {
                        mActiveState = States.Destination;
                    }
                    break;
            }
        }

        private Vector2 ObjectInSight(List<CollectableItem> item, List<CryoChamber> cc)
        {
            foreach (var sup in mSupervisor)
            {
                foreach (var it in item)
                {
                    if (Vector2.Distance(it.Position, sup.MidPoint) < sup.Vision)
                    {
                        return it.Position;
                    }
                }

                foreach (var cr in cc)
                {
                    if (Vector2.Distance(cr.mPosition, sup.MidPoint) < sup.Vision)
                    {
                        return cr.mPosition;
                    }
                }
            }
            return Vector2.Zero;
        }

        private Vector2 EnemyInRange(HashSet<ICharacter> players)
        {
            foreach (var sup in mSupervisor)
            {
                var play = ClosestTarget(players, sup);
                if (play == null) { return Vector2.Zero; }
                if (Vector2.Distance(play.MidPoint, sup.MidPoint) < sup.Vision)
                {
                    return play.MidPoint;
                }
            }
            return Vector2.Zero;
        }

        private static ICharacter ClosestTarget(HashSet<ICharacter> players, ICharacter busy)
        {
            if (!players.Any())
            {
                return null;
            }
            var closestEnemy = players.First();
            // Search for the closest enemy.
            foreach (var play in players)
            {
                if (Vector2.Distance(play.MidPoint, busy.MidPoint) <
                    Vector2.Distance(closestEnemy.MidPoint, busy.MidPoint))
                {
                    closestEnemy = play;
                }
            }

            return closestEnemy;
        }

        // remove dead enemies from mBusy and mSupervisor
        void UpdateSurvivors()
        {
            foreach(var en in mBusy)
            {
                if (!mUnoccupied.Contains(en))
                {
                    mBusy.Remove(en);
                    mBusyManager.Remove(en);
                    break;
                }
            }

            foreach (var sup in mSupervisor)
            {
                if (!mUnoccupied.Contains(sup))
                {
                    mSupervisor.Remove(sup);
                    mSuperManager.Remove(sup);
                    break;
                }
            }
        }

        public void Update(GameTime g, BattleModeState bms)
        {
            mSecondsPassed += g.ElapsedGameTime.Milliseconds / 1000f;
            UpdateSurvivors();
            KiBrain(bms.mPlayerCharacters, bms.Items, bms.CryoChambers);
            foreach (var manager in mBusyManager)
            {
                manager.Value.UpdateActionList(g, bms.mPlayerCharacters, bms.DistractionObjects);
            }

            foreach (var manager in mFreeManager)
            {
                manager.Value.UpdateActionList(g, bms.mPlayerCharacters, bms.DistractionObjects);
            }

            foreach (var manager in mSuperManager)
            {
                manager.Value.UpdateActionList(g, bms.mPlayerCharacters, bms.DistractionObjects);
            }
        }
    }
}

