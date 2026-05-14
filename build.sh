dotnet run --project ./CakeBuild/CakeBuild.csproj -- "$@"
# Find the latest fieldsofcranberries zip file and copy it to the Mods directory
latest_zip=$(ls -t ~/Documents/projects/VintageStoryMods/fields_of_cranberries_vsmod/Releases/fieldsofcranberries_*.zip 2>/dev/null | head -n1)
if [ -n "$latest_zip" ]; then
    cp "$latest_zip" ~/.config/VintagestoryData/Mods/
    echo "Copied $(basename "$latest_zip") to Mods directory"
else
    echo "No fieldsofcranberries zip file found in Releases directory"
fi
