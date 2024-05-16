using Robocode;
using Robocode.Util;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Collections;

namespace CAP4053.Student
{
    public class SalmonBot : TeamRobot
    {
        private bool movingForward;
        private bool isNearWall;
        private bool enemyLowEnergy = false;
        private bool timeSwitch = false;

        public override void Run()
        {
            BodyColor = Color.White;
            GunColor = Color.White;
            RadarColor = Color.Black;
            BulletColor = Color.Red;
            ScanColor = Color.Red;

            IsAdjustRadarForRobotTurn = true;
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;

            if (X <= 50 || Y <= 50 || BattleFieldWidth - X <= 50 || BattleFieldHeight - Y <= 50)
            {
                isNearWall = true;
            }
            else
            {
                isNearWall = false;
            }

            SetAhead(40000);
            SetTurnRadarRight(360);
            movingForward = true;
            
            while (true)
            {
                if (X > 50 && Y > 50 && BattleFieldWidth - X > 50 && BattleFieldHeight - Y > 50 && isNearWall) {
                    isNearWall = false;
                }
                if (X <= 50 || Y <= 50 || BattleFieldWidth - X <= 50 || BattleFieldHeight - Y <= 50)
                {
                    if (!isNearWall)
                    {
                        reverseDirection(40000);
                        isNearWall = true;
                    }
                }

                if (RadarTurnRemaining == 0.0)
                {
                    SetTurnRadarRight(360);
                }

                Execute();
            }
        }


        public void reverseDirection(int num)
        {
            if (movingForward)
            {
                SetBack(num);
                movingForward = false;
            }
            else
            {
                SetAhead(num);
                movingForward = true;
            }
        }

        /// <summary>
        ///   onScannedRobot:  We have a target.  Go get it.
        /// </summary>
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            if (IsTeammate(e.Name))
                return;

            double absBearing = Heading + e.Bearing;
            double bearingFromGun = Utils.NormalRelativeAngleDegrees(absBearing - GunHeading);
            double bearingFromRadar = Utils.NormalRelativeAngleDegrees(absBearing - RadarHeading);

            /*if (e.Energy < 20)
            {
                enemyLowEnergy = true;
                if (movingForward)
                {
                    SetTurnRight(NormalRelativeAngleDegrees(e.Bearing + 80));
                }
                else
                {
                    SetTurnRight(NormalRelativeAngleDegrees(e.Bearing + 100));
                }
                SetAhead(40000);
                return;
            }*/

            if (e.Distance < 500 && movingForward && !isNearWall /*&& e.Energy >= 20*/)
            {
                reverseDirection(40000);
            }

            if (Time % 20 == 0)
            {
                timeSwitch = !timeSwitch;
            }

            /*if (e.Energy < 20 && Energy > e.Energy)
            {
                SetTurnRight(NormalRelativeAngleDegrees(e.Bearing));
                enemyLowEnergy = true;
            }
            else*/ if (!timeSwitch)
            {
                if (movingForward)
                {
                    SetTurnRight(Utils.NormalRelativeAngleDegrees(e.Bearing + 80));
                }
                else
                {
                    SetTurnRight(Utils.NormalRelativeAngleDegrees(e.Bearing + 100));
                }
            }
            else
            {
                if (movingForward)
                {
                    SetTurnLeft(Utils.NormalRelativeAngleDegrees(e.Bearing + 80));
                }
                else
                {
                    SetTurnLeft(Utils.NormalRelativeAngleDegrees(e.Bearing + 100));
                }
            }

            //if (e.Distance < 1000 && movingForward && !isNearWall)
            //{
                //SetBack(40000);
            //}
            //else if (e.Distance >= 500 && reversed)
            //{
                //reverseDirection();
                //reversed = false;
            //}

            if (Math.Abs(bearingFromGun) <= 4)
            {
                SetTurnGunRight(bearingFromGun);
                SetTurnRadarRight(bearingFromRadar);

                if (GunHeat == 0 && Energy > .2 && !enemyLowEnergy)
                {
                    Fire(Math.Min(4.5 - Math.Abs(bearingFromGun) / 2 - e.Distance / 250, Energy - .1));
                }
            }
            else
            {
                SetTurnGunRight(bearingFromGun);
                SetTurnRadarRight(bearingFromRadar);
            }

            if (bearingFromGun == 0)
            {
                Scan();
            }
        }

        public override void OnHitByBullet(HitByBulletEvent e)
        {
                reverseDirection(40000);
        }

        public override void OnHitRobot(HitRobotEvent e)
        {
            if (e.IsMyFault) {
                reverseDirection(40000);
            }
        }

        public override void OnHitWall(HitWallEvent e)
        {
            reverseDirection(40000);
        }

        public override void OnWin(WinEvent e)
        {
            // Victory dance
            TurnRight(36000);
        }
    }
}
