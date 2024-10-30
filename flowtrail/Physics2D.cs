using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace flowtrail
{
    internal class Physics2D
    {
        public static bool GetCollision(Physics2D colliderA, Physics2D colliderB)
        {
            Vector2 updatedPositionColliderA = (colliderA._position + colliderA._velocity);
            Vector2 updatedPositionColliderB = (colliderB._position + colliderB._velocity);

            bool collision
                = (MathF.Abs(updatedPositionColliderA.X - updatedPositionColliderB.X)
                <= (colliderA._radiusOfBoundingSphere + colliderB._radiusOfBoundingSphere)
                && (MathF.Abs(updatedPositionColliderA.Y - updatedPositionColliderB.Y)
                <= (colliderA._radiusOfBoundingSphere + colliderB._radiusOfBoundingSphere)));

            return collision;
        }

        public int ID { get; set; }

        public Vector2 _position { get; set; }

        public float _velocityDecayStrength { get; set; }

        public Vector2 _direction { get; set; }

        public Vector2 _velocity { get; set; }

        public float _velocityClampRadius { get; set; }

        public float _gravity { get; set; }

        public float _radiusOfBoundingSphere { get; set; }

        public List<Physics2D> _collisions;
        
        public List<int> _indicesOfInvalidCollisions;

        public bool _updateCollision { get; set; }

        public float _maxSpeedX { get; set; }
        public float _maxSpeedY { get; set; }

        public float _minX { get; set; }
        public float _maxX { get; set; }
        public float _minY { get; set; }
        public float _maxY { get; set; }

        public int _testFlag1 { get; set; }

        public int _testFlag2 { get; set; }

        public int _testFlag3 { get; set; }

        public int _testFlag4 { get; set; }


        public Physics2D(bool updateCollision = false)
        {
            _updateCollision = updateCollision;

            _position = new Vector2(0.0f, 0.0f);
            _velocityDecayStrength = 60.0f;
            _direction = new Vector2(1.0f, 0.0f);
            _velocity = new Vector2(0.0f, 0.0f);
            _velocityClampRadius = 1.5f;
            _gravity = 11.0f;
            _maxSpeedX = 8.0f;
            _maxSpeedY = 6.0f;

            _testFlag1 = 0;
            _testFlag2 = 0;
            _testFlag3 = 0;
            _testFlag4 = 0;

            _collisions = new List<Physics2D>();
            _indicesOfInvalidCollisions = new List<int>();
        }

        public void Move(Vector2 direction, float force)
        {
            _velocity = new Vector2(
                _velocity.X + direction.X * force
                , _velocity.Y + direction.Y * force);
        }

        public void Update(float deltaTime)
        {
            // horizontal decay of velocity
            if (_velocity.X != 0.0f)
            {
                if (_velocity.X > _velocityClampRadius)
                {
                    _velocity = new Vector2(
                        _velocity.X - _velocityDecayStrength * deltaTime
                        , _velocity.Y);
                }
                else if (_velocity.X < -_velocityClampRadius)
                {
                    _velocity = new Vector2(
                        _velocity.X + _velocityDecayStrength * deltaTime
                        , _velocity.Y);
                }
                else
                {
                    _velocity = new Vector2(
                        0.0f, _velocity.Y);
                }

                if (_velocity.X > _maxSpeedX)
                {
                    _velocity = new Vector2(_maxSpeedX, _velocity.Y);
                }
                else if (_velocity.X < -_maxSpeedX)
                {
                    _velocity = new Vector2(-_maxSpeedX, _velocity.Y);
                }
            }

            // enable this and disable gravity for top down gameplay
            //if (_velocity.Y != 0.0f)
            //{
            //    if (_velocity.Y > _velocityClampRadius)
            //    {
            //        _velocity = new Vector2(
            //            _velocity.X
            //            , _velocity.Y - _velocityDecayStrength * deltaTime);
            //    }
            //    else if (_velocity.Y < -_velocityClampRadius)
            //    {
            //        _velocity = new Vector2(
            //            _velocity.X
            //            , _velocity.Y + _velocityDecayStrength * deltaTime);
            //    }
            //    else
            //    {
            //        _velocity = new Vector2(
            //            _velocity.X, 0.0f);
            //    }

            //    if (_velocity.Y > _maxSpeedX)
            //    {
            //        _velocity = new Vector2(_velocity.X, _maxSpeedX);
            //    }
            //    else if (_velocity.Y < -_maxSpeedX)
            //    {
            //        _velocity = new Vector2(_velocity.X, -_maxSpeedX);
            //    }
            //}

            if(_velocity.Y < -_maxSpeedY)
            {
                _velocity = new Vector2(_velocity.X, -_maxSpeedY);
            }

            // apply gravity to velocity
            _velocity = new Vector2(
                _velocity.X
                , _velocity.Y + _gravity * deltaTime);

            // only enabled for the player
            // needs more work
            // implement as onCollision, postCollision, exitCollision 

            _testFlag1 = 0;
            _testFlag2 = 0;
            _testFlag3 = 0;
            _testFlag4 = 0;

            // disable gravity and test 2D movement
            if (_updateCollision)
            {
                for(int i = 0; i < _collisions.Count; i++)
                {
                    Physics2D collider = _collisions[i];

                    Vector2 updatedPosition = _position + _velocity;

                    float minX = updatedPosition.X - _radiusOfBoundingSphere;
                    float maxX = updatedPosition.X + _radiusOfBoundingSphere;

                    float minY = updatedPosition.Y - _radiusOfBoundingSphere;
                    float maxY = updatedPosition.Y + _radiusOfBoundingSphere;

                    float colliderMinY = collider._position.Y - collider._radiusOfBoundingSphere;
                    float colliderMaxY = collider._position.Y + collider._radiusOfBoundingSphere;

                    float colliderMinX = collider._position.X - collider._radiusOfBoundingSphere;
                    float colliderMaxX = collider._position.X + collider._radiusOfBoundingSphere;

                    bool overlapX = maxX >= colliderMinX && minX <= colliderMaxX;
                    bool overlapY = maxY >= colliderMinY && minY <= colliderMaxY;

                    _minX = minX;
                    _minY = minY;

                    _maxX = maxX; 
                    _maxY = maxY;

                    float radiusSum = collider._radiusOfBoundingSphere + _radiusOfBoundingSphere;

                    float prevFlag1 = _testFlag1;

                    //_testFlag1 = Convert.ToInt16(overlapX && overlapY);
                    _testFlag1 = Convert.ToInt16(MathF.Abs(updatedPosition.X - collider._position.X) <= radiusSum 
                        && MathF.Abs(updatedPosition.Y - collider._position.Y) <= radiusSum);

                    //if(prevFlag1 != _testFlag1)
                    //{
                    //    Debug.WriteLine("Collider " + i + " Flag1: " + _testFlag1);
                    //}

                    // there is a collision
                    if(_testFlag1 == 1)
                    {
                        Vector2 distance = _position - collider._position;
                        distance = new Vector2(MathF.Abs(distance.X), MathF.Abs(distance.Y));

                        if (_velocity.Y > 0)
                        {
                            if (updatedPosition.Y < collider._position.Y)
                            {
                                if (distance.X < radiusSum)
                                {
                                    _velocity = new Vector2(_velocity.X, 0.0f);
                                }
                            }
                        }

                        if (_velocity.Y < 0)
                        {
                            if (updatedPosition.Y > collider._position.Y)
                            {
                                if (distance.X < radiusSum)
                                {
                                    _velocity = new Vector2(_velocity.X, 0.0f);
                                }
                            }
                        }

                        if (_velocity.X > 0)
                        {
                            if (updatedPosition.X < collider._position.X)
                            {
                                if (distance.Y < radiusSum)
                                {
                                    _velocity = new Vector2(0.0f, _velocity.Y);
                                }
                            }
                        }

                        if (_velocity.X < 0)
                        {
                            if (updatedPosition.X > collider._position.X)
                            {
                                if (distance.Y < radiusSum)
                                {
                                    _velocity = new Vector2(0.0f, _velocity.Y);
                                }
                            }
                        }
                    }

                    //_testFlag1 = Convert.ToInt16(updatedPosition.X + _radiusOfBoundingSphere >= collider._position.X - _radiusOfBoundingSphere);
                    //_testFlag2 = Convert.ToInt16(updatedPosition.X - _radiusOfBoundingSphere <= collider._position.X + _radiusOfBoundingSphere);
                    //_testFlag3 = Convert.ToInt16(overlapX); //Convert.ToInt16(updatedPosition.Y - _radiusOfBoundingSphere >= collider._position.Y + _radiusOfBoundingSphere);
                    //_testFlag4 = Convert.ToInt16(overlapY);

                    //if((_testFlag1 == 1 || _testFlag2 == 1) && overlapX == false && overlapY == false)
                    //{
                        //_velocity = new Vector2(0.0f, _velocity.Y);
                    //}
                    
                    //if (overlapX && overlapY)
                    //{
                        //_velocity = new Vector2(_velocity.X, 0.0f);

                        //if (updatedPosition.Y <= colliderMinY)
                        //{
                        //    _velocity = new Vector2(_velocity.X, 0.0f);
                        //}
                        //else if (_position.Y >= colliderMaxY)
                        //{
                        //    //Move(new Vector2(0.0f, 1.0f), MathF.Abs(_velocity.Y - _gravity) );
                        //    //_velocity = new Vector2(_velocity.X, 0.0f);
                        //}
                        //if (_position.X < _collisions[i]._position.X)
                        //{
                        //    _velocity = new Vector2(0.0f, _velocity.Y);
                        //}
                        //else if (_position.X > _collisions[i]._position.X)
                        //{
                        //    _velocity = new Vector2(0.0f, _velocity.Y);
                        //}
                    //}
                }
            }

            // there is a better way of doing this
            for (int i = _indicesOfInvalidCollisions.Count - 1; i > -1; i--)
            {
                _collisions.RemoveAt(_indicesOfInvalidCollisions[i]);
            }

            _indicesOfInvalidCollisions.Clear();

            // apply velocity to position
            _position += _velocity;
        }
    }
}
