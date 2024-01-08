# Contributing

First off, thanks for wishing to contribute!

Not only will this benefit the other developers of this application, but contributing to a structured open-source project will expose you to the disciplines of software development and prepare you for doing work in the professional world

## Creating an Issue

Before you start fixing problems or adding value, it may be at your behest to find something to complain about first!

Loads of issues have likely been filed before you show up, but now is your opportunity to submit your request to the proverbial complaint box

Existing templates for issues are present so you can create an issue in a guided manner, and using one is as simple as [going here](https://github.com/ExoKomodo/video-game-design/issues/new/choose)

## Fixing an issue

Assign yourself to the issue and start hacking away as detailed below.


## Creating a branch

I cannot stress this enough, but **DO NOT USE A [FORK](https://docs.github.com/en/get-started/quickstart/fork-a-repo)**

Forks are frustrating to work with, as they are:
- difficult to know what is actually being worked on
- difficult to keep in-sync

## Creating a pull request

The moment you create a branch to work on an issue, you should go ahead and create a `Draft pull request`

By having a draft pull request, you signal any CI to start automatically building and testing your changes, preparing you for merge!

## Syncing changes from main

Say the `main` branch is updated before you can merge your change in, no biggie

You have 2 options:
- Update the branch in the Github UI
<img width="916" alt="Screenshot 2023-03-29 at 8 03 15 AM" src="https://user-images.githubusercontent.com/17893076/228582048-f242d46c-e8af-462b-abb3-2cb6e505decb.png">
- Update the branch locally

```shell
git checkout main
git pull
git checkout my-branch
# Now pick one of these following two
## 1. rebase
git rebase main
## 2. merge
git merge main
```

## Closing a pull request

Once the CI is happy, and a developer has reviewed your changes, go ahead and merge your PR!

