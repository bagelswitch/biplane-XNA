float4x4 g_mWorldViewProjection : WORLDVIEWPROJ;    // World * View * Projection matrix


struct VS_OUTPUT
{
  float4 Position   : POSITION;   // vertex position 
  float4 Color      : COLOR;
};


VS_OUTPUT RenderSceneVS( float4 vPos : POSITION, 
                         float4 vColor : COLOR )
{
  VS_OUTPUT Output;
  
  // Transform the position from object space to homogeneous projection space
  Output.Position = mul(vPos, g_mWorldViewProjection);
 
  // And color
  Output.Color = vColor;

  return Output;
}


struct PS_OUTPUT
{
  float4 RGBColor : COLOR0;  // Pixel color    
};


PS_OUTPUT RenderScenePS( VS_OUTPUT In ) 
{ 
  PS_OUTPUT Output;

  Output.RGBColor = In.Color;
  Output.RGBColor[3] = 0.1;

  return Output;
}



technique RenderScene
{
  pass P0
  {
    AlphaBlendEnable = true;
    ZEnable = false;
    CullMode = None;
//    FogVertexMode = NONE;
//    FogEnable = FALSE;
    VertexShader = compile vs_1_1 RenderSceneVS();
    PixelShader  = compile ps_1_1 RenderScenePS(); 
  }
}
