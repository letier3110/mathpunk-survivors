import bpy

C = bpy.context
scn = C.scene

# add home directory to path
home = bpy.path.abspath("//")
 
initialPath = home + "output/"
textureDirectory = home + "textures/"

textures = [
  "grass_uv_example.png",
  "cube_uv_4.png",
  "dust_uv.png",
  "dust_uv_1.png",
  "dust_uv_2.png",
  "dust_uv_3.png",
  "water_uv.png",
]

i = 1

for texture in textures:
  filepath = initialPath + str(i) + ".png"
  texturePath = textureDirectory + texture
  imgNode = scn.objects["MainCub"].active_material.node_tree.nodes.get("Image Texture")
  imgNode.image = bpy.data.images.load(texturePath)
  scn.render.filepath = filepath
  bpy.ops.render.render(write_still=True)
  i += 1

