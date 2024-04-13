using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class WeaponFollowV1 : MonoBehaviour
    {
        private const float DelayTime = 0.1f;
        private float _delayTimer = 0f;
        private Vector3 _delayFollowPosition = Vector3.zero;
        private Vector3 _weaponPosition = Vector3.zero;
        
        private const int SIZE = 7;
        private int _readIdx = 0;
        private int _recordIdx = 0;
        private bool _rotationInit = true;
        [SerializeField] private float _rotationSpeed = 10;
        private Quaternion _cacheRotation = Quaternion.identity;
        private Quaternion[] _passRotation = new Quaternion[SIZE];
        

        [SerializeField] private Transform _parentTrans;

        private void Awake()
        {
            _weaponPosition = transform.position;
            _cacheRotation = transform.rotation;
        }


        // Update is called once per frame
        void Update()
        {
            if (_delayTimer <= 0f)
            {
                RecordCurrentState();
                _delayTimer = DelayTime;
            }
            _delayTimer -= Time.deltaTime;
            _weaponPosition = Vector3.Lerp(_weaponPosition, _delayFollowPosition, Time.deltaTime * _rotationSpeed);
            var y = _parentTrans.worldToLocalMatrix.MultiplyPoint(_weaponPosition).y;
            y = Mathf.Clamp(y, -0.2f, 0.2f);
            transform.localPosition = new (0,y,0);

            var rotation = _parentTrans.rotation;
            _passRotation[_recordIdx].Set(rotation.x, rotation.y, rotation.z, rotation.w);
            _recordIdx = (_recordIdx + 1) % SIZE;
            if (_rotationInit && _recordIdx == 0)
            {
                _rotationInit = false;
            }
            if (!_rotationInit)
            {
                _cacheRotation = Quaternion.Lerp(_cacheRotation, _passRotation[_readIdx], Time.deltaTime * _rotationSpeed);
                _readIdx = (_readIdx + 1) % SIZE;
                transform.rotation = _cacheRotation;
            }
        }

        void RecordCurrentState()
        {
            var localToWorldMatrix = _parentTrans.localToWorldMatrix;
            _delayFollowPosition = localToWorldMatrix.MultiplyPoint(Vector3.zero);
        }
    }
}
