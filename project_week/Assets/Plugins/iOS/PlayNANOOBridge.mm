//
//  PlayNANOOBridge.m
//  PlayNANOOUnity
//
//  Created by LimJongHyun on 2017. 6. 1..
//  Copyright © 2017년 LimJongHyun. All rights reserved.
//

#import "PlayNANOOBridge.h"

extern "C" UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

@implementation PlayNANOOBridge

-(id)init:(NSString *)gameID serviceKey:(NSString *)serviceKey secretKey:(NSString *)secretKey version:(NSString *)version {
    self = [super init];
    viewController = UnityGetGLViewController();
    plugin = [[PNPlugin alloc] initWithPluginInfo:gameID serviceKey:serviceKey secretKey:secretKey version:[version integerValue]];

    return self;
}

-(void)_pnInit{}

-(void)_pnSetUniqueUserID:(NSString *)value {
    [plugin setUniqueUserID:value];
}

-(void)_pnSetNickName:(NSString *)value {
    [plugin setUserName:value];
}

-(void)_pnSetLanguage:(NSString *)value {
    [plugin setUserLanguage:value];
}

-(void)_pnOpenBanner {
    [plugin openBanner:viewController];
}

-(void)_pnOpenForum {
    [plugin openForm:viewController];
}

-(void)_pnOpenForumView:(NSString *)url {
    [plugin openForumView:viewController url:url];
}

-(void)_pnOpenScreenshot:(NSString *)value {}

-(void)_pnHelpDeskOptional:(NSString *)key value:(NSString *)value {
    [plugin helpDeskOptional:key value:value];
}

-(void)_pnOpenHelpDesk {
    [plugin openHelpDesk:viewController];
}

-(void)_pnAccessEventRequestValues {
    NSString *inviteCode = [plugin inviteCode];
    UnitySendMessage([@"PlayNANOO" UTF8String], "sendMessageInviteCode", [[NSString stringWithFormat:@"%@", inviteCode] UTF8String]);

    NSString *inviteUserRequestCode = [plugin inviteUserRequestCode];
    UnitySendMessage([@"PlayNANOO" UTF8String], "sendMessageInviteUserRequestCode", [[NSString stringWithFormat:@"%@", inviteUserRequestCode] UTF8String]);
    
    NSString *idfa = [PNUtils idfa];
    UnitySendMessage([@"PlayNANOO" UTF8String], "sendMessageADID", [[NSString stringWithFormat:@"%@", idfa] UTF8String]);
    
    UnitySendMessage([@"PlayNANOO" UTF8String], "sendMessageAccessEventRequest", [@"Null" UTF8String]);
}

-(void)_pnOpenShare:(NSString *)title message:(NSString *)message {
	[plugin openShare:viewController title:title message:message];
}

//UnitySendMessage([@"PlayNANOO" UTF8String], "OnIDFA", [[NSString stringWithFormat:@"%@", uniqueID] UTF8String]);
@end

extern "C" {
    PlayNANOOBridge *bridge;
    
    void _pnInit(const char* gameID, const char* serviceKey, const char* secretKey, const char* version) {
        bridge = [[PlayNANOOBridge alloc] init:[NSString stringWithUTF8String:gameID] serviceKey:[NSString stringWithUTF8String:serviceKey] secretKey:[NSString stringWithUTF8String:secretKey] version:[NSString stringWithUTF8String:version]];
    }

    void _pnSetUniqueUserID(const char* value) {
        [bridge _pnSetUniqueUserID:[NSString stringWithUTF8String:value]];
    }

    void _pnSetNickName(const char* value) {
        [bridge _pnSetNickName:[NSString stringWithUTF8String:value]];
    }

    void _pnSetLanguage(const char* value) {
        [bridge _pnSetLanguage:[NSString stringWithUTF8String:value]];
    }

    void _pnOpenBanner() {
        [bridge _pnOpenBanner];
    }

    void _pnOpenForum() {
        [bridge _pnOpenForum];
    }

    void _pnOpenForumView(const char* url) {
        [bridge _pnOpenForumView:[NSString stringWithUTF8String:url]];
    }

    void _pnOpenScreenshot(const char* value) {
        [bridge _pnOpenScreenshot:[NSString stringWithUTF8String:value]];
    }

    void _pnHelpDeskOptional(const char* key, const char* value) {
        [bridge _pnHelpDeskOptional:[NSString stringWithUTF8String:key] value:[NSString stringWithUTF8String:value]];
    }

    void _pnOpenHelpDesk() {
        [bridge _pnOpenHelpDesk];
    }

    void _pnAccessEventRequestValues() {
        [bridge _pnAccessEventRequestValues];
    }

    void _pnOpenShare(const char* title, const char* message) {
    	[bridge _pnOpenShare:[NSString stringWithUTF8String:title] message:[NSString stringWithUTF8String:message]];
    }

    // void _pnInviteCode(const char* value) {
    //     [bridge _pnInviteCode];
    // }
}
