//
//  ViewController.m
//  Slovo
//
//  Created by Dev Test on 5/31/14.
//  Copyright (c) 2014 Amsada. All rights reserved.
//

// https://www.youtube.com/watch?v=P2yaZXn4MU0
// todo: http://www.mysamplecode.com/2013/08/ios-uisearchbar-with-uitableview-example.html

#import "ViewController.h"
#import "Manager.h"
#import "Vocabulary.h"

@interface ViewController ()
<UITableViewDataSource, UITableViewDelegate, UISearchBarDelegate>
@property (weak, nonatomic) IBOutlet UISearchBar *searchBar;
@property (weak, nonatomic) IBOutlet UITableView *tableView;
@property (nonatomic, strong) NSMutableArray* initialCities;
@property (nonatomic, strong) NSMutableArray* filteredCities;
@property BOOL isFiltered;
@end

@implementation ViewController
@synthesize searchBar, tableView, initialCities, filteredCities, isFiltered;
- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view, typically from a nib.
    
    // Alloc initial array
    initialCities = [[NSMutableArray alloc] initWithObjects:@"London", @"Seattle", @"Redmond", nil];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - UITableView Delegate methods
- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
}

- (NSInteger) tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    if (isFiltered == YES) {
        return filteredCities.count;
    }
    else {
        return initialCities.count;
    }
}

- (UITableViewCell *) tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    static NSString *cellIdentifier = @"Cell";
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier: cellIdentifier];
    if (cell == Nil) {
        cell = [[UITableViewCell alloc] initWithStyle:
                UITableViewCellStyleDefault reuseIdentifier:cellIdentifier];
    }
    
    if (isFiltered == YES) {
        cell.textLabel.text = [filteredCities objectAtIndex:indexPath.row];
    }
    else {
        cell.textLabel.text = [initialCities objectAtIndex:indexPath.row];
    }
    
    return cell;
}


- (NSInteger) tableView:(UITableView *)tableView numberOfSectionsInTableView:(NSInteger)section {
    return 1;
}


#pragma mark UISearchBarDelegate methods
- (void)searchBar:(UISearchBar *)searchBar textDidChange:(NSString *)searchText {
    if (searchText.length == 0) {
        // set boolean flag
        isFiltered = NO;
    }
    else {
        isFiltered = YES;
        filteredCities = [[NSMutableArray alloc] init];
        
        // fast enumeration
        for (NSString * cityName in initialCities) {
            NSRange cityNameRange = [cityName rangeOfString:searchText options:NSCaseInsensitiveSearch];
            if (cityNameRange.location != NSNotFound) {
                [filteredCities addObject:cityName];
            }
        }
    }
    
    // reload table view
    [tableView reloadData];
}

- (void)searchBarSearchButtonClicked:(UISearchBar *)searchBar
{
    // [searchBar resignFirstResponder];
    Manager *manager = [Manager instance];
    Vocabulary *vocabulary = manager.vocabulary;
    [vocabulary Load];
}

@end
