float4x4 world;
float4x4 view;
float4x4 projection;

float timer;

struct VertexShaderInput
{
    float4 Position : POSITION; //in object space
};

struct VertexShaderOutput
{
    float4 Position : POSITION0; //in projection space
    float3 Normal : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput IN)
{
	VertexShaderOutput OUT;

	// will be used for color (reflection-esque)
	OUT.Normal = float3(0, 0, 0);
	float little = cos((IN.Position.y / 1) + (timer*1));
	float big = cos((IN.Position.x / 10) + (timer*3.374));
	if(little > big) {
		OUT.Normal.z = little;
	} else {
		OUT.Normal.z = big;
	}

	// mesh waves
	IN.Position.z += ((sin((IN.Position.y / 1) + (timer*1)) * 4) + (sin((IN.Position.x / 10) + (timer*3.374)) * 3));
	float4 worldPosition = mul(IN.Position, world);
	float4 viewPosition = mul(worldPosition, view);
	OUT.Position = mul(viewPosition, projection);

    return OUT;
}

float4 PixelShaderFunction(VertexShaderOutput IN) : COLOR0
{
    // TODO: add your pixel shader code here.

    float4 watCol = float4(0.2, 0.4, 0.6, 0.5);
    float4 reflCol = float4(0,0,0,0);
    reflCol = float4(IN.Normal.z,IN.Normal.z,1/IN.Normal.z,0.0);
	reflCol = normalize(reflCol);
	watCol = watCol + (reflCol/15);
  
    return watCol;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();

        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
