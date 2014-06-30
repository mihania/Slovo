//
//  Manager.m
//  Slovo
//
//  Created by Dev Test on 6/1/14.
//  Copyright (c) 2014 Amsada. All rights reserved.
//

#import "Manager.h"
#import "Vocabulary.h"

@implementation Manager
static Manager *gInstance = NULL;



+ (Manager *)instance
{
    @synchronized(self)
    {
        if (gInstance == NULL)
            gInstance = [[self alloc] init];
    }
    
    return(gInstance);
}

- (id) init {
    self = [super init];
    if (self != nil) {
        self.vocabulary = [[Vocabulary alloc] init];
    }

    return self;
}


@end
