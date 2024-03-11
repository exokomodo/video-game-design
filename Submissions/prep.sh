#!/bin/sh
#!/usr/bin/env bash

#                          INSTRUCTIONS
#                          ============
#
#                         !!READ BEFORE!!
#
# *Intended for MAC users only*
# This script will copy over only necessary files, create UNTESTED file in Windows/, and compress it all for you.
#
# Save this inside a folder for submissions (e.g. "CS_6457/Submissions/prep.sh").
# The unity project folder should be at the same level as this submissions folder.
#   (e.g. the project folder named "CS4455_M1_Support" should be in the same parent folder: "CS_6457/CS4455_M1_Support").
#
# Before building, don't forget to update the ProjectSettings > Player > ProductName of your project
# to be the same as your AssignmentTitle, and update `assignmentCode` param in the Auditor > Auditor GameObject.
#
# You should have both OSX and Windows builds and the readme file (Burdell_G_m2_readme.txt/md) before running this script.
# The Windows build should be in <ProjectName>/Build/Windows/<AssignmentTitle>/,
#   which is automatically done when you create a build in Unity with the
#   appropriate AssignmentTitle (e.g. Burdell_G_m2) in the folder Build/Windows.
#
# Run this by calling `./prep.sh` from the submissions folder, and
#   enter appropriate project name and assignment title as prompted.
# Before uploading the zip file, ALWAYS double check by unzipping it, running the game, and checking Audit.

printf "What is the unity project folder name? (e.g. CS4455_M1_Support) -> "
read MDIR

printf "What is your assignment title? (e.g. Burdell_G_m2) -> "
read MNAME

MDIR="../$MDIR"
DDIR="$MNAME"

echo "Prepping assignment $MNAME from dir $MDIR..."

mkdir $DDIR
mkdir $DDIR/Build

# Copy over OSX executable of the correct version
mkdir $DDIR/Build/OSX
cp -R $MDIR/Build/OSX/$MNAME.app $DDIR/Build/OSX/$MNAME.app

# Copy over Windows executable of the correct version, and add an empty "UNTESTED" file
mkdir $DDIR/Build/Windows
cp -R $MDIR/Build/Windows/$MNAME/** $DDIR/Build/Windows/
touch $DDIR/Build/Windows/UNTESTED

# Copy over rest of the important folders & README file
cp -R $MDIR/Assets $DDIR/
cp -R $MDIR/Packages $DDIR/
cp -R $MDIR/ProjectSettings $DDIR/
cp $MDIR/*_readme.txt $DDIR/

# Zip it up!
zip -r -X -q $MNAME.zip $MNAME

# Remove uncompressed folder
rm -r $DDIR

echo "Done! Created $MNAME.zip. Don't forget to unzip and test!"

