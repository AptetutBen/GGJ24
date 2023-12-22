#!/bin/bash
urlencode() {
  python3 -c 'import html,sys; print(html.escape(sys.argv[1], sys.argv[2]))' \
    "$1" "$urlencode_safe"
}

ExpiryTime=$(($(date +%s) + 7776000))
ProjectName="GGJ2024"

ScriptDirectory=$(dirname "$0")

Platform=$1
ArtifactPath=$2
Extension=$3
BundleID=$4
bundleVersion=$5
smallIconPath=$6
largeIconPath=$7

FileName=$ProjectName-$Platform-$(date +'%Y-%m')$Extension

rm "$FileName" || true
mv "$ArtifactPath" "$FileName"

ArtifactURL="https://builds.connectedplay.io/$ProjectName/$FileName"
InstallURL="https://builds.connectedplay.io/$ProjectName/install_ios.html"
ManifestURL="https://builds.connectedplay.io/$ProjectName/ios_manifest.plist"
Icon57URL="https://builds.connectedplay.io/$ProjectName/icon57.png"
Icon512URL="https://builds.connectedplay.io/$ProjectName/icon512.png"

PrivateKey=~/CloudFront/private_connectedplay_builds_key.pem

echo $FileName

Policy="{
    \"Statement\": [
        {
            \"Resource\": \"https://builds.connectedplay.io/$ProjectName/*\",
            \"Condition\": {
                \"DateLessThan\": {
                    \"AWS:EpochTime\": $ExpiryTime
                }
            }
        }
    ]
}"

Base64Policy=$(echo "$Policy" | tr -d "\n" | tr -d " \t\n\r" | openssl base64 | tr -- '+=/' '-_~' | tr -d "\n" | tr -d " \t\n\r")
SignedPolicy=$(echo "$Policy" | tr -d "\n" | tr -d " \t\n\r" | openssl sha1 -sign $PrivateKey | openssl base64 | tr -- '+=/' '-_~'| tr -d "\n" | tr -d " \t\n\r") 

SignedArtifactURL="$ArtifactURL?Policy=$Base64Policy&Signature=$SignedPolicy&Key-Pair-Id=$CloudFrontKeyPairID"
SignedInstallURL="$InstallURL?Policy=$Base64Policy&Signature=$SignedPolicy&Key-Pair-Id=$CloudFrontKeyPairID"
SignedManifestURL="$ManifestURL?Policy=$Base64Policy&Signature=$SignedPolicy&Key-Pair-Id=$CloudFrontKeyPairID"
SignedIcon57URL="$Icon57URL?Policy=$Base64Policy&Signature=$SignedPolicy&Key-Pair-Id=$CloudFrontKeyPairID"
SignedIcon512URL="$Icon512URL?Policy=$Base64Policy&Signature=$SignedPolicy&Key-Pair-Id=$CloudFrontKeyPairID"

# The plist file requires urlencoding
EncodedArtifactURL=$(urlencode "$SignedArtifactURL")
EncodedIcon57URL=$(urlencode "$SignedIcon57URL")
EncodedIcon512URL=$(urlencode "$SignedIcon512URL")
# The manifest url requires html encoding for ? & and = characters
EncodedManifestURL=$(echo $SignedManifestURL | gsed -e 's@[\&]@%26@g' | gsed -e 's@[\?]@%3F@g' | gsed -e 's@[\=]@%3D@g')

DownloadURL="itms-services:\/\/\?action=download-manifest\&url="$(printf '%s\n' "$EncodedManifestURL" | gsed -e 's@[\/&]@\\&@g')

echo "$EncodedManifestURL"
echo "$EncodedArtifactURL"

FileSize=$(ls -lah $FileName | awk -F " " {'print $5'})

echo "Replacing {build_number}"
gsed -i 's@{build_number}@'$RevisionNumber'@g' "$ScriptDirectory/install.html"
echo "Replacing {project_name}"
gsed -i 's@{project_name}@'$ProjectName'@g' "$ScriptDirectory/install.html"
echo "Replacing {platform_name}"
gsed -i 's@{platform_name}@'$Platform'@g' "$ScriptDirectory/install.html"
echo "Replacing {build_time}"
gsed -i "s@{build_time}@$(date)@g" "$ScriptDirectory/install.html"
echo "Replacing {file_size}"
gsed -i 's@{file_size}@'$FileSize'@g' "$ScriptDirectory/install.html"
echo "Replacing {download_url}"
gsed -i 's@{download_url}@'$DownloadURL'@g' "$ScriptDirectory/install.html"

echo "Replacing {ipa_url}"
gsed -i 's@{ipa_url}@'$(printf '%s\n' "$EncodedArtifactURL" | gsed -e 's@[\/&]@\\&@g')'@g' "$ScriptDirectory/ios_manifest.plist"
echo "Replacing {application_name}"
gsed -i 's@{application_name}@'$ProjectName'@g' "$ScriptDirectory/ios_manifest.plist"
echo "Replacing {bundle_id}"
gsed -i 's@{bundle_id}@'$BundleID'@g' "$ScriptDirectory/ios_manifest.plist"

echo "Replacing {bundle_version}"
gsed -i 's@{bundle_version}@'$bundleVersion'@g' "$ScriptDirectory/ios_manifest.plist"
echo "Replacing {icon_57_url}"
gsed -i 's@{icon_57_url}@'$(printf '%s\n' "$EncodedIcon57URL" | gsed -e 's@[\/&]@\\&@g')'@g' "$ScriptDirectory/ios_manifest.plist"
echo "Replacing {icon_512_url}"
gsed -i 's@{icon_512_url}@'$(printf '%s\n' "$EncodedIcon512URL" | gsed -e 's@[\/&]@\\&@g')'@g' "$ScriptDirectory/ios_manifest.plist"

aws s3 cp "$FileName" s3://builds.connectedplay.io/$ProjectName/$FileName --profile $AWSProfile
aws s3 cp "$ScriptDirectory/install.html" s3://builds.connectedplay.io/$ProjectName/install_ios.html --profile $AWSProfile
aws s3 cp "$ScriptDirectory/ios_manifest.plist" s3://builds.connectedplay.io/$ProjectName/ios_manifest.plist --profile $AWSProfile
aws s3 cp "$smallIconPath" s3://builds.connectedplay.io/$ProjectName/icon57.png --profile $AWSProfile
aws s3 cp "$largeIconPath" s3://builds.connectedplay.io/$ProjectName/icon512.png --profile $AWSProfile

aws cloudfront create-invalidation \
    --distribution-id $CloudFrontDistID \
    --paths "/$ProjectName/*" --profile $AWSProfile

GitVersionHash=$(git rev-parse --short HEAD)
DateNow=$(date '+%Y-%m-%d %H:%M:%S')
DiscordPostData="{\
    \"content\": \"New [$Platform build #$GitVersionHash ($FileSize)]($SignedInstallURL) uploaded at $DateNow\"
}"
curl -i -H "Accept: application/json" -H "Content-Type:application/json" \
-X POST --data "$DiscordPostData" "$DiscordWebhook"