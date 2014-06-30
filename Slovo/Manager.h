//
//  Manager.h
//  Slovo
//
//  Created by Dev Test on 6/1/14.
//  Copyright (c) 2014 Amsada. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Vocabulary.h"

@interface Manager : NSObject

+ (Manager *)instance;

+ (id) init;

@property Vocabulary *vocabulary;

@end
