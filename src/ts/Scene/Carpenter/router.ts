import * as GLP from 'glpower';

import { Entity } from "~/ts/libs/framework/Entity";
import { Content1 } from '../Entities/Content1';
import { DustParticles } from '../Entities/DustParticles';
import { FluidParticles } from '../Entities/FluidParticles';
import { Floor } from '../Entities/Floor';

export const router = ( node: GLP.BLidgeNode ) => {

	if ( node.name == "Content1" ) {

		return new Content1();

	} else if ( node.name == "DustParticles" ) {

		return new DustParticles();

	} else if ( node.name == "FluidParticles" ) {

		// return new FluidParticles();

	} else if ( node.name == "Floor" ) {

		return new Floor();

	}

	return new Entity();

};
