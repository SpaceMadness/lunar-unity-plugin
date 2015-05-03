//
//  LunarAssertActionSheet.m
//  LunarPluginAssert
//
//  Created by Alex Lementuev on 2/22/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarAssertViewController.h"

static const float kButtonHeight = 30;
static const float kIndentHor = 16;
static const float kIndentVer = 20;

@interface LunarAssertViewController () <UITableViewDataSource, UITableViewDelegate>

@end

@implementation LunarAssertViewController

- (instancetype)initWithTitle:(NSString *)title
{
    self = [super init];
    if (self)
    {
    }
    
    return self;
}

- (void)viewDidLoad
{
    UIButton *button = [self createButtonTitle:@"Continue" action:@selector(onContinueButton:)];
    button.frame = CGRectMake(kIndentHor, kIndentVer, 200, kButtonHeight);
    [self.view addSubview:button];
}

#pragma mark -
#pragma mark Actions

- (void)onContinueButton:(id)sender
{
}

#pragma mark -
#pragma mark Helpers

- (UIButton *)createButtonTitle:(NSString *)title action:(SEL)action
{
    UIButton *button = [UIButton buttonWithType:UIButtonTypeRoundedRect];
    [button setTitle:title forState:UIControlStateNormal];
    [button addTarget:self action:action forControlEvents:UIControlEventTouchUpInside];
    return button;
}

#pragma mark -
#pragma mark UITableViewDataSource

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return 0;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return nil;
}

#pragma mark -
#pragma mark UITableViewDelegate


@end
