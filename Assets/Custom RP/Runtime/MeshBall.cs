using UnityEngine;

public class MeshBall : MonoBehaviour {

    // 预先缓存 Shader 属性的 ID，避免在 Update 中使用字符串导致 CPU 性能开销
    static int baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private Mesh mesh = default;

    [SerializeField]
    private Material material = default;

    // GPU Instancing 单次批量绘制的最大硬件限制缓冲区（受限于 CBuffer 的 64KB 上限）
    private const int MAX_INSTANCE_COUNT = 1023;

    // 在 CPU 端开辟的数组，分别存放 1023 个实例的 变换矩阵 和 颜色数据
    private Matrix4x4[] matrices = new Matrix4x4[MAX_INSTANCE_COUNT];
    private Vector4[] baseColors = new Vector4[MAX_INSTANCE_COUNT];

    // Unity 材质属性块，用于在不破坏合批的前提下，向 GPU 批量传递自定义材质属性（数组）
    private MaterialPropertyBlock block;

    void Awake () {
        // 在游戏启动时，一次性在 CPU 端随机生成 1023 个球体的位置与颜色
        for (int i = 0; i < matrices.Length; i++) {
            // 随机在半径为 10 的球体内分布，旋转为默认，缩放为 1
            matrices[i] = Matrix4x4.TRS(
                Random.insideUnitSphere * 10f, 
                Quaternion.identity, 
                Vector3.one
            );
            
            // 随机生成 RGB 颜色，A（Alpha通道）固定为 1.0f
            baseColors[i] = new Vector4(
                Random.value, 
                Random.value, 
                Random.value, 
                1f
            );
        }
    }

    void Update () {
        // 健壮性检查：确保面板上已经赋值了 Mesh 和 Material，否则不执行渲染
        if (mesh == null || material == null) {
            return;
        }

        // 延迟初始化 MaterialPropertyBlock
        // 放在这里不仅性能好，还能确保在编辑器环境下“代码热重载（Hot Reload）”后数据不丢失
        if (block == null) {
            block = new MaterialPropertyBlock();
            // 将 CPU 端的 Vector4 数组一次性打包绑定到属性块中
            block.SetVectorArray(baseColorId, baseColors);
        }

        // 核心魔法：绕过 GameObject 架构，直接向底层图形 API 发送单次合批绘制指令
        // 传入：网格、子网格索引(0)、材质、矩阵数组、绘制数量(1023)、包含了颜色数组的属性块
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, MAX_INSTANCE_COUNT, block);
    }
}