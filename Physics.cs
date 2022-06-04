using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity_Simulation
{
    public static class Physics
    {
        public static bool running = false;
        public static float simTime = 0, simPrecision = 1, simPeriod, simSpeed = 1, gravConstant = 100;
        public static int simFrequency = 0;
        public static List<Body> bodyStates = new List<Body>();

        private readonly static List<Body> bodies = new List<Body>();
        private static int frequencyCounter = 0;


        public static void StartSimulation()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Stopwatch secondwatch = new Stopwatch();
            secondwatch.Start();

            while (true)
            {
                if (secondwatch.Elapsed.TotalSeconds >= 1)
                {
                    secondwatch.Restart();
                    simFrequency = frequencyCounter;
                    frequencyCounter = 0;
                }
                    
                
                TimeSpan time = stopwatch.Elapsed;
                simPeriod = 1 / 50f / simPrecision / simSpeed;
                if (running && (time.TotalSeconds >= simPeriod))
                {
                    frequencyCounter++;
                    stopwatch.Restart();
                    SimulateTick();
                }
            }
        }

        private static void SimulateTick()
        {
            simTime += simPeriod * simSpeed;

            //calculate gravity acceleration
            foreach (Body body1 in bodies)
            {
                foreach (Body body2 in bodies)
                {
                    if (body1 != body2) Accelerate(body1, body2);
                }
            }

            //move bodies
            foreach (Body body in bodies) body.Update(simPrecision);

            //check for collisions
            HashSet<Body> toBeRemoved = new HashSet<Body>();
            foreach (Body body1 in bodies)
            {
                foreach (Body body2 in bodies)
                {
                    if ((body1 != body2) && !toBeRemoved.Contains(body1) && !toBeRemoved.Contains(body2) && body1.OverlapsWith(body2))
                    {
                        if (body1.mass < body2.mass)
                        {
                            body2.CollidesWith(body1);
                            toBeRemoved.Add(body1);
                        }
                        else
                        {
                            body1.CollidesWith(body2);
                            toBeRemoved.Add(body2);
                        }
                    }
                }
            }
            foreach (Body body in toBeRemoved)
            {
                bodies.Remove(body);
            }

            bodyStates = CopyBodies(bodies);
        }

        private static List<Body> CopyBodies(List<Body> bodies)
        {
            List<Body> newBodyList = new List<Body>();
            foreach (Body body in bodies)
            {
                Body newBody = new Body(body.pos, body.velocity, body.radius, body.mass, body.texture);
                newBodyList.Add(newBody);
            }
            return newBodyList;
        }

        private static void Accelerate(Body sourceBody, Body targetBody)
        {
            Vector2 localize = sourceBody.pos;
            sourceBody.pos -= localize;
            targetBody.pos -= localize;

            Vector2 direction = sourceBody.pos - targetBody.pos;
            float distance = direction.Length();
            direction.Normalize();
            float magnitude = gravConstant * sourceBody.mass / (float)Math.Pow(distance, 2);

            targetBody.acceleration += direction * magnitude;

            sourceBody.pos += localize;
            targetBody.pos += localize;
        }

        public static void CreateBody(Body body)
        {
            bodies.Add(body);
        }
    }
}
