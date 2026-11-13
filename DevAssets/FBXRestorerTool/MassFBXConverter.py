import bpy
import os
import math

# A python script to mass restore the Club Penguin Island FBX models from a .glb that gets exported from the UnityGLTF plugin (https://github.com/KhronosGroup/UnityGLTF). This script will not restore rigged models with bones.

# Folder where the script resides
script_dir = os.path.dirname(os.path.realpath(__file__))
output_dir = os.path.join(script_dir, "FBX Models")
os.makedirs(output_dir, exist_ok=True)

glb_files = [f for f in os.listdir(script_dir) if f.lower().endswith(".glb")]

for file in glb_files:
    input_path = os.path.join(script_dir, file)
    output_path = os.path.join(output_dir, os.path.splitext(file)[0] + ".fbx")

    # Reset Blender scene
    bpy.ops.wm.read_factory_settings(use_empty=True)

    # Import GLB
    bpy.ops.import_scene.gltf(filepath=input_path)

    # Set the material
    mat_name = "Default-Material"
    if mat_name in bpy.data.materials:
        mat = bpy.data.materials[mat_name]
    else:
        mat = bpy.data.materials.new(name=mat_name)

    # Select the object
    bpy.ops.object.select_all(action='SELECT')

    # STEP 1: Reset the transforms for the selected object
    for obj in bpy.context.selected_objects:
        obj.location = (0.0, 0.0, 0.0)
        obj.scale = (1.0, 1.0, 1.0)
        obj.rotation_mode = 'QUATERNION'
        obj.rotation_quaternion = (1.0, 0.0, 0.0, 0.0)

    # STEP 2: Switch the object to the XYZ Euler mode
    for obj in bpy.context.selected_objects:
        obj.rotation_mode = 'XYZ'

    # STEP 3: Set X rotation to 270°, leave Y/Z as default
    for obj in bpy.context.selected_objects:
        obj.rotation_euler.x = math.radians(270)

    # STEP 4: Apply the rotation for the selected object
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=False)

    # STEP 5: Assign material and Face Corner Byte Color
    for obj in bpy.context.selected_objects:
        if obj.type == 'MESH':
            # Material
            obj.data.materials.clear()
            obj.data.materials.append(mat)

            # Face Corner Byte Color
            mesh = obj.data
            color_name = "Col"
            if color_name not in mesh.color_attributes:
                color_layer = mesh.color_attributes.new(
                    name=color_name,
                    type='BYTE_COLOR',
                    domain='CORNER'
                )

                white = (1.0, 1.0, 1.0, 1.0)
                for i in range(len(color_layer.data)):
                    color_layer.data[i].color = white
            else:
                print(f"Skipped adding '{color_name}' for {obj.name} (already exists).")

    # STEP 6: Export FBX
    bpy.ops.export_scene.fbx(
        filepath=output_path,
        use_selection=False,
        apply_unit_scale=True,
        bake_space_transform=False,
        global_scale=1.0
    )

    print(f"Converted {file} - {output_path}")
