#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

// 1. 最先引入官方的通用宏定义库
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

// 2. 引入我们自己的 UnityInput 变量声明
#include "UnityInput.hlsl"

// 3. 建立“认亲宣言”（宏定义映射）
// 告诉官方库：你代码里的 UNITY_MATRIX_M 对应的就是我这里的 unity_ObjectToWorld
#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_I_V unity_MatrixInvV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_PREV_MATRIX_M unity_prev_MatrixM
#define UNITY_PREV_MATRIX_I_M unity_prev_MatrixIM
#define UNITY_MATRIX_P glstate_matrix_projection

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

// 4. 最后引入官方的空间变换库（它会自动使用上面我们映射好的宏）
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

#endif