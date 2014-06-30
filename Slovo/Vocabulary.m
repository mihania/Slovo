//
//  Vocabulary.m
//  Slovo
//
//  Created by Dev Test on 6/1/14.
//  Copyright (c) 2014 Amsada. All rights reserved.
//

#import "Vocabulary.h"

@implementation Vocabulary
NSString *name = @"rom_rus";
bool loaded = NO;
NSMutableArray *myArray;
int wordsCount = 185704;
char firstByte = 'a';

-(void) Load {
    if (loaded == NO) {
        myArray = [[NSMutableArray alloc] initWithCapacity:wordsCount];
        [self LoadFile];
    }
}

-(void) LoadFile {
    NSURL *path = [[NSBundle mainBundle] URLForResource:name withExtension:@"idx"];
    NSString *stringPath = [path absoluteString]; //this is correct
    
    //you can again use it in NSURL eg if you have async loading images and your mechanism
    //uses only url like mine (but sometimes i need local files to load)
    NSData *fileData = [NSData dataWithContentsOfURL:[NSURL URLWithString:stringPath]];
    
    char *buffer = (char *)[fileData bytes];
    NSUInteger length = [fileData length];
 
    int index = 0;
    int previous = 0;
    while (index < length) {
        if (buffer[index] != '\0') {
            index++;
        }
        else {
            // NSString
        }
    }
}

@end
