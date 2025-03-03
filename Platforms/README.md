## 📱 Switching from Windows, Linux, or macOS to Android or iOS

When switching platforms in Unity, you might need to adjust certain assets and prefabs due to unexpected behavior. Follow the steps below to ensure a smooth transition.  
**Note:** These steps must also be repeated if you switch back to **standalone** platforms (Windows, Linux, or macOS).  

### ⚠️ Known Issues on Mobile
- **Fishing Minigame:** Currently broken on mobile platforms. Investigating the cause — stay tuned for updates!  
- **Lightmaps:** Every lightmap needs to be **rebaked** with a low resolution of **512** for mobile compatibility.  

### 🔄 Asset and Prefab Adjustments

1. **`InputBarContainer.asset`**  
   - **Node Prefab:** Replace `InputBarContainerPC.prefab` with `InputBarContainer.prefab`.  

2. **`WorldAreaControls.asset`**  
   - **Issue:** Check if anything appears as `Missing`.  
   - **Fix:** If missing, replace with `Controls.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  

3. **`VertLayout.asset`**  
   - **Issue:** Check if anything appears as `Missing`.  
   - **Fix:** If missing, replace with `VertLayout.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  

4. **`SettingsCanvas.asset`**  
   - **Issue:** Check if anything appears as `Missing`.  
   - **Fix:** If missing, replace with `SettingsCanvas.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  

5. **`LocomotionJoystickInput.asset`**  
   - **Issue:** Check if anything appears as `Missing` at the **Joystick Prefab** field.  
   - **Fix:** If missing, replace with `JoyStickPanel.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  
