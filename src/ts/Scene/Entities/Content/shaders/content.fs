#include <common>
#include <packing>
#include <frag_h>

#include <sdf>
#include <noise>
#include <rotate>
#include <re>

uniform vec3 cameraPosition;
uniform mat4 modelMatrixInverse;
uniform float uTime;
uniform float uTimeSeq;

vec2 D( vec3 p ) {

	vec3 pp = p;

	// pp.xz *= rotate( uTime * 0.1 );

	vec2 d = vec2( 99999.0, 0.0 );

	// vec2 d = vec2( sdSphere( pp, 0.03 ), 0.0 );
	float t = uTime * 0.5;

	float radius = 0.5 + fbm(p * 0.5 + fbm3(p * 0.8 - uTime * 0.1) * 1.0 + uTime * 0.1) * 0.7;

	// radius += smoothstep( 0.3, 0.7, fbm(p * 0.5) ) *0.5;

	d = add( d, vec2( sdSphere( pp, radius ), 1.0 ) );
	
	return d;

}

vec3 N( vec3 pos, float delta ){

    return normalize( vec3(
		D( pos ).x - D( vec3( pos.x - delta, pos.y, pos.z ) ).x,
		D( pos ).x - D( vec3( pos.x, pos.y - delta, pos.z ) ).x,
		D( pos ).x - D( vec3( pos.x, pos.y, pos.z - delta ) ).x
	) );
	
}

void main( void ) {

	#include <frag_in>

	vec3 rayPos = ( modelMatrixInverse * vec4( vPos, 1.0 ) ).xyz;
	vec3 rayDir = normalize( ( modelMatrixInverse * vec4( normalize( vPos - cameraPosition ), 0.0 ) ).xyz );
	vec2 dist = vec2( 0.0 );
	bool hit = false;

	vec3 normal;
	
	for( int i = 0; i < 64; i++ ) { 

		dist = D( rayPos );		
		rayPos += dist.x * rayDir;

		if( dist.x < 0.01 ) {

			normal = N( rayPos, 0.0001 );

			hit = true;
			break;

		}
		
	}

	if( dist.y == 1.0 ) {
		
		outRoughness = 1.0;
		outMetalic = 0.0;
		outColor.xyz = vec3( 1.0, 1.0, 1.0 );
		
	} else if( dist.y == 0.0 ) {

		outEmission =  vec3( 1.0, 0.7, 0.7 ) * smoothstep( 0.0, 1.0, dot( normal, -rayDir ) );
		
	} 

	outNormal = normalize(modelMatrix * vec4( normal, 0.0 )).xyz;

	if( !hit ) discard;

	outColor = vec4( 0.0, 0.0, 0.0, 1.0 );

	#ifdef IS_FORWARD

		for( int i = 0; i < 8; i++ ) {

			outColor.x += texelFetch( uDeferredTexture, ivec2( gl_FragCoord.xy + normal.xy * (100.0 + float(i) * 10.0 ) ), 0 ).r;
			outColor.y += texelFetch( uDeferredTexture, ivec2( gl_FragCoord.xy + normal.xy * (130.0 + float(i) * 10.0 ) ), 0 ).g;
			outColor.z += texelFetch( uDeferredTexture, ivec2( gl_FragCoord.xy + normal.xy * (160.0 + float(i) * 10.0 ) ), 0 ).b;

		}

		outColor.xyz /= 8.0;

	#endif

	outPos = ( modelMatrix * vec4( rayPos, 1.0 ) ).xyz;

	#include <frag_out>

}