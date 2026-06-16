using System.Collections.Generic;
using Unity.VisualScripting;
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
        public int Bounce;
    }
    
    public ActiveWeapon.EWeaponSlot WeponSlot;
    public bool IsFiring = false;
    [Tooltip("발사 빈도, 초당 몇발")]
    public int FireRate = 25;
    public float BulletSpeed = 1000.0f;
    [Tooltip("탄 낙하(스피드 낮게 드랍 높게)")]
    public float BulletDrop = 0.0f;
    [Tooltip("탄 튕김 빈도")]
    public int MaxBounces = 0;
    public int AmmoCount;
    public int MaxAmmoSize;

    public ParticleSystem[] MuzzleFlashs; // Emission 끈상태
    public ParticleSystem HitEffect;
    public TrailRenderer BulletTracerEffect;
    public AnimationClip WeaponAnimClip;        // Weapon의 Idle 클립
    public string WeaponName;

    public Transform RaycastOrigin;
    public Transform RaycastDestination; // 실제 레이 부딪힌 타겟위치(CrossHairTarget)
    
    public GameObject Magazine;
    
    [SerializeField]
    private LayerMask _hitMask;

    [HideInInspector] public WeaponRecoil Recoil;
    private Ray _ray;
    private RaycastHit _hitInfo;
    private float _accumulatedTime;

    private List<Bullet> _bulletList = new List<Bullet>();
    private float maxLifetime = 3.0f;

    private void Awake()
    {
        Recoil = GetComponent<WeaponRecoil>();
        
    }
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
        bullet.Bounce = MaxBounces;

        return bullet;
    }
    public void StartFiring()
    {
        IsFiring = true;
        _accumulatedTime = 0.0f;
        Recoil.Reset();
        //FireBullet();
    }

    public void UpdateWeapon(float p_deltaTime)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartFiring();
        }
        if (IsFiring)
        {
            UpdateFiring(Time.deltaTime);
        }
        UpdateBullets(Time.deltaTime);
        if (Input.GetButtonUp("Fire1"))
        {
            StopFiring();
        }
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

        if (Physics.Raycast(_ray, out _hitInfo, distance, _hitMask))
        {
            //Debug.DrawLine(_ray.origin, _hitInfo.point, Color.red, 1.0f);
            HitEffect.transform.position = _hitInfo.point;
            HitEffect.transform.forward = _hitInfo.normal;
            HitEffect.Emit(1);

            p_bullet.Tracer.transform.position = _hitInfo.point;
            p_bullet.Time = maxLifetime;

            // Bullet Ricochet (MaxBounces만큼 반사되어 튕겨나가끔 하는 기능) 특별한 무기 있을 시
            if(p_bullet.Bounce > 0)
            {
                p_bullet.Time = 0;
                p_bullet.InitialPosion = _hitInfo.point;
                p_bullet.InitialVelocity = Vector3.Reflect(p_bullet.InitialVelocity, _hitInfo.normal);
                p_bullet.Bounce--;
            }

            // Collision impulse
            var rb2d = _hitInfo.collider.GetComponent<Rigidbody>();
            if(rb2d)
            {
                rb2d.AddForceAtPosition(_ray.direction * 20, _hitInfo.point, ForceMode.Impulse);
            }
        }
        else
            p_bullet.Tracer.transform.position = p_end;
    }
    private void FireBullet()
    {
        if (AmmoCount <= 0) return;

        AmmoCount--;
        foreach (var firing in MuzzleFlashs)
        {
            firing.Emit(1);
        }

        // BulletSpeed 안넣으면 에러 발생
        Vector3 velocity = (RaycastDestination.position - RaycastOrigin.position).normalized * BulletSpeed;
        var bullet = CreateBullet(RaycastOrigin.position, velocity);
        _bulletList.Add(bullet);

        Recoil.GenerateRecoil(WeaponName);

        Debug.DrawLine(RaycastOrigin.position, RaycastDestination.position, Color.red, 1f);
    }
    public void StopFiring()
    {
        IsFiring = false;
    }
}
