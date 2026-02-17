# Fixing "Could not load file or assembly 'Newtonsoft.Json, Version=13.0.0.0'"

This error usually means the runtime cannot find the exact Newtonsoft.Json
assembly version your app was built against, or a different version is being
loaded at runtime.

## Quick checks

1. **Confirm a package reference exists**
   - SDK-style projects:
     - Run `dotnet add package Newtonsoft.Json` to add the latest package.
   - Non-SDK projects:
     - Ensure a NuGet package reference exists and is restored.

2. **Ensure the assembly is copied to output**
   - Build the app and verify `Newtonsoft.Json.dll` is in the output folder
     (for example, `bin/Debug` or `bin/Release`).
   - If you are publishing, confirm the publish output also includes the DLL.

3. **Add a binding redirect (for .NET Framework)**
   - Add this to `app.config` or `web.config`:
     ```
     <runtime>
       <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
         <dependentAssembly>
           <assemblyIdentity name="Newtonsoft.Json"
                             publicKeyToken="30ad4fe6b2a6aeed"
                             culture="neutral" />
           <bindingRedirect oldVersion="0.0.0.0-13.0.0.0"
                            newVersion="13.0.0.0" />
         </dependentAssembly>
       </assemblyBinding>
     </runtime>
     ```
   - You can also enable auto-redirects in the project file:
     ```
     <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
     <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
     ```

4. **Clean and restore**
   - Run `dotnet clean` and `dotnet restore` (or reinstall packages in Visual
     Studio) to ensure the correct version is restored.

## Common causes

- **Version mismatch**: one dependency pulls an older Newtonsoft.Json version
  while another expects 13.0.0.0.
- **Missing binding redirect** in .NET Framework apps.
- **Excluded assets**: `PrivateAssets` or `ExcludeAssets` prevent the DLL from
  being copied to the output.
- **Stale binaries**: old DLLs in the output folder or GAC.

## If it still fails

- Check which version is being loaded by logging `Assembly.Load("Newtonsoft.Json")`
  and verifying the resolved path.
- Remove conflicting versions from the output directory and rebuild.
