# Old Camera production artwork

Generated with the built-in image_gen tool on September 27, 2026.

Project asset: Assets/Resources/Art/OldCamera/Camera_Atlas.png

The original transparent PNG is preserved as one atlas. Unity slices the chassis, flash housing,
side panel (used twice), and lens at runtime. No offline image modifications were applied.
Slice coordinates are documented in OldCameraLevelBootstrap.cs in source-image pixels.
The importer preserves alpha, disables mipmaps and compression, and allows a 2048-pixel texture.

## Generation prompt

Use case: stylized-concept. Create a single production sprite atlas for a warm workshop mobile restoration puzzle: antique film camera components. Genuine transparent background. Square image, exact 2 by 2 equal grid of invisible cells. No text, labels, grid lines, screws, fasteners, logos, cast shadows outside silhouettes, or backdrop. Each component centered in its own cell with clear transparent margin of 8 percent of cell on all sides; nothing crosses cell borders. All components perfectly front-facing orthographic, symmetrical where applicable, no perspective tilt. Premium tactile stylized 2.5D game rendering, warm upper-left light, rich dark brown pebbled leather, aged brushed brass rim, restrained wear, polished edges, subtle muted teal optical glass. TOP LEFT: camera main rectangular chassis only, width to height ratio 1.45, rounded brass outer rim, dark leather face, completely plain face with no lens and no top flash mechanism, suitable as base beneath other parts. TOP RIGHT: separate long horizontal brass top plate with small center black-glass viewfinder protrusion and raised rectangular flash housing toward right; overall width to height approximately 3.2; flash glass dark cream ribbed glass, no lens or camera body. BOTTOM LEFT: one narrow vertical rectangular leather grip/film-door panel, height to width 2.7, slim brass rim, rounded corners; will be reused for both sides of camera. BOTTOM RIGHT: single perfectly circular antique camera lens assembly, dark concentric barrel, brushed brass outer ring, deeply recessed muted teal glass with elegant small upper-left glint; no writing. The four components should look like matching pieces of one collectible camera. Large crisp shapes readable at phone size. Keep all spare space actually transparent.
