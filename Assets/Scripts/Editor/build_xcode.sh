#!/bin/bash

#  $1 = xcode project path
#  $2 = Development team
#  $3 = Provisioning profile name
#  $4 = exportOptions plist

# Example:
# sh Game/Assets/Scripts/Editor/build_xcode.sh \
# "Builds/ios/XcodeProject/Unity-iPhone.xcodeproj" \
# "ABC123" \
# "PROVISIONING PROFILE NAME" \
# "CODE SIGN IDENTITY" \
# "Game/Assets/Scripts/Editor/exportOptions.plist"
echo "xcodebuild clean archive"
xcodebuild -project "$1" -scheme Unity-iPhone -allowProvisioningUpdates clean archive -archivePath archive DEVELOPMENT_TEAM="$2" PROVISIONING_PROFILE_SPECIFIER_APP="$3" CODE_SIGN_IDENTITY_APP="$4"

echo "xcodebuild export archive"
xcodebuild -exportArchive -archivePath archive.xcarchive -exportOptionsPlist "$6" -exportPath export CODE_SIGN_IDENTITY_APP="$5"
