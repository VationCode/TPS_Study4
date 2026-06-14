using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float Time;
        public Vector3 InitialPosion;
        public Vector3 InitialVelocity;
        public TrailRenderer Tracer;
    }
    public bool IsFiring = false;
    [Tooltip("발사 빈도, 초당 몇발")]
    public int FireRate = 25;
    public float BulletSpeed = 1000.0f;
    public float BulletDrop = 0.0f;
    public ParticleSystem[] MuzzleFlashs; // Emission 끈상태
    public ParticleSystem HitEffect;
    public TrailRenderer BulletTracerEffect;
    public AnimationClip WeaponAnimClip;

    public Transform RaycastOrigin;
    public Transform RaycastDestination; // 실제 레이 부딪힌 타겟위치(CrossHairTarget)

    private Ray _ray;
    private RaycastHit _hitInfo;
    private float _accumulatedTime;

    private List<Bullet> _bulletList = new List<Bullet>();
    private float maxLifetime = 3.0f;
    Vector3 GetPosition(Bullet p_bullet)
    {
        // pos + velocity * time + 0.5 * gravity * time *time = 포물 (낙하 탄도, 속력 낮게 드랍 높이면 떨어짐)
        Vector3 gravity = Vector3.down * BulletDrop;
        return (p_bullet.InitialPosion) + (p_bullet.InitialVelocity * p_bullet.Time) +
            (0.5f * gravity * p_bullet.Time * p_bullet.Time);
    }

    Bullet CreateBullet(Vector3 p_pos, Vector3 p_velocity)
    {
        Bullet bullet = new Bullet();
        bullet.InitialPosion = p_pos;
        bullet.InitialVelocity = p_velocity;
        bullet.Time = 0.0f;
        bullet.Tracer = Instantiate(BulletTracerEffect, p_pos, Quaternion.identity);
        bullet.Tracer.AddPosition(p_pos);
        return bullet;
    }
    public void StartFiring()
    {
        IsFiring = true;
        _accumulatedTime = 0.0f;
        FireBullet();
    }
    public void UpdateFiring(float p_deltaTime)
    {
        _accumulatedTime += p_deltaTime;
        // FireRate = 10일 때 0.1초, 
        float fireInterval = 1.0f / FireRate;

        while (_accumulatedTime >= fireInterval)
        {
            FireBullet();
            _accumulatedTime -= fireInterval;
        }
    }
    public void UpdateBullets(float p_delaTime)
    {
        SimulateBullets(p_delaTime);
        DestroyBullets();
    }

    private void SimulateBullets(float p_deltaTime)
    {
        _bulletList.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.Time += p_deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void DestroyBullets()
    {
        // [문제 발생]
        // FireBullet에서 velocity계산시 BulletSpeed 누락을 하게되면
        // 총알이 거의 안 움직임
        // Bullet/Tracer가 오래 살아있음
        // AutoDestruct가 먼저 Tracer를 삭제
        // 코드가 삭제된 Tracer에 접근
        // MissingReferenceException

        // 트레일랜더러에서 Autodestruct 사용시 이코드
        //_bulletList.RemoveAll(bullet => bullet.Time >= maxLifetime);

        // Autodestruct 사용 안하고 직접 제어
        _bulletList.RemoveAll(bullet =>
        {
            if (bullet.Time >= maxLifetime)
            {
                if (bullet.Tracer != null)
                {
                    Destroy(bullet.Tracer.gameObject);
                }

                return true;
            }

            return false;
        });
    }
    private void RaycastSegment(Vector3 p_start, Vector3 p_end, Bullet p_bullet)
    {
        Vector3 direction = p_end - p_start;
        float distance = direction.magnitude;
        _ray.origin = p_start;
        _ray.direction = direction;

        if (Physics.Raycast(_ray, out _hitInfo, distance))
        {
            //Debug.DrawLine(_ray.origin, _hitInfo.point, Color.red, 1.0f);
            HitEffect.transform.position = _hitInfo.point;
            HitEffect.transform.forward = _hitInfo.normal;
            HitEffect.Emit(1);

            p_bullet.Tracer.transform.position = _hitInfo.point;
            p_bullet.Time = maxLifetime;
        }
        else
            p_bullet.Tracer.transform.position = p_end;
    }
    private void FireBullet()
    {
        foreach (var firing in MuzzleFlashs)
        {
            firing.Emit(1);
        }

        // BulletSpeed 안넣으면 에러 발생
        Vector3 velocity = (RaycastDestination.position - RaycastOrigin.position).normalized * BulletSpeed;
        var bullet = CreateBullet(RaycastOrigin.position, velocity);
        _bulletList.Add(bullet);
    }
    public void StopFiring()
    {
        IsFiring = false;
    }
}
