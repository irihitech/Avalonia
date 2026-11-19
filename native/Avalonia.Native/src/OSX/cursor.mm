#include "common.h"
#include "cursor.h"

class CursorFactory : public ComSingleObject<IAvnCursorFactory, &IID_IAvnCursorFactory>
{
    ComPtr<Cursor> arrowCursor = comnew<Cursor>([NSCursor arrowCursor]);
    ComPtr<Cursor> crossCursor = comnew<Cursor>([NSCursor crosshairCursor]);
    ComPtr<Cursor> resizeUpCursor = comnew<Cursor>([NSCursor resizeUpCursor]);
    ComPtr<Cursor> resizeDownCursor = comnew<Cursor>([NSCursor resizeDownCursor]);
    ComPtr<Cursor> resizeUpDownCursor = comnew<Cursor>([NSCursor resizeUpDownCursor]);
    ComPtr<Cursor> dragCopyCursor = comnew<Cursor>([NSCursor dragCopyCursor]);
    ComPtr<Cursor> openHandCursor = comnew<Cursor>([NSCursor openHandCursor]);
    ComPtr<Cursor> dragLinkCursor = comnew<Cursor>([NSCursor dragLinkCursor]);
    ComPtr<Cursor> pointingHandCursor = comnew<Cursor>([NSCursor pointingHandCursor]);
    ComPtr<Cursor> contextualMenuCursor = comnew<Cursor>([NSCursor contextualMenuCursor]);
    ComPtr<Cursor> IBeamCursor = comnew<Cursor>([NSCursor IBeamCursor]);
    ComPtr<Cursor> resizeLeftCursor = comnew<Cursor>([NSCursor resizeLeftCursor]);
    ComPtr<Cursor> resizeRightCursor = comnew<Cursor>([NSCursor resizeRightCursor]);
    ComPtr<Cursor> resizeWestEastCursor = comnew<Cursor>([NSCursor resizeLeftRightCursor]);
    ComPtr<Cursor> operationNotAllowedCursor = comnew<Cursor>([NSCursor operationNotAllowedCursor]);
    ComPtr<Cursor> closedHandCursor = comnew<Cursor>([NSCursor closedHandCursor]);
    ComPtr<Cursor> IBeamVerticalCursor = comnew<Cursor>([NSCursor IBeamCursorForVerticalLayout]);
    ComPtr<Cursor> noCursor = comnew<Cursor>([NSCursor arrowCursor], true);

    std::map<AvnStandardCursorType, ComPtr<Cursor>> s_cursorMap =
    {
        { CursorArrow, arrowCursor },
        { CursorAppStarting, arrowCursor },
        { CursorWait, arrowCursor },
        { CursorTopLeftCorner, crossCursor },
        { CursorTopRightCorner, crossCursor },
        { CursorBottomLeftCorner, crossCursor },
        { CursorBottomRightCorner, crossCursor },
        { CursorCross, crossCursor },
        { CursorSizeAll, crossCursor },
        { CursorSizeNorthSouth, resizeUpDownCursor},
        { CursorSizeWestEast, resizeWestEastCursor},
        { CursorTopSide, resizeUpCursor },
        { CursorUpArrow, resizeUpCursor },
        { CursorBottomSize, resizeDownCursor },
        { CursorDragCopy, dragCopyCursor },
        // AppKit has no drag-move cursor; dragCopy/dragLink are arrow-based, so the
        // plain arrow conveys "drop will move" — openHand is reserved for Grab.
        { CursorDragMove, arrowCursor },
        { CursorDragLink, dragLinkCursor },
        { CursorHand, pointingHandCursor },
        { CursorHelp, contextualMenuCursor },
        { CursorIbeam, IBeamCursor },
        { CursorLeftSide, resizeLeftCursor },
        { CursorRightSide, resizeRightCursor },
        { CursorNo, operationNotAllowedCursor },
        { CursorNone, noCursor },
        // No native NSCursor equivalent — closest standard shape
        { CursorSizeNorthWestSouthEast, crossCursor },
        { CursorSizeNorthEastSouthWest, crossCursor },
        { CursorGrab, openHandCursor },
        { CursorGrabbing, closedHandCursor },
        { CursorZoomIn, arrowCursor },
        { CursorZoomOut, arrowCursor },
        { CursorCell, crossCursor },
        { CursorVerticalText, IBeamVerticalCursor },
        { CursorContextualMenu, contextualMenuCursor },
        { CursorNoDrop, operationNotAllowedCursor },
        { CursorAllScroll, openHandCursor },
        { CursorColResize, resizeWestEastCursor },
        { CursorRowResize, resizeUpDownCursor }
    };

public:
    FORWARD_IUNKNOWN()
    
    virtual HRESULT GetCursor (AvnStandardCursorType cursorType, ComPtr<IAvnCursor>& retOut) override
    {
        retOut = s_cursorMap[cursorType];
        return S_OK;
    }
    
    virtual HRESULT CreateCustomCursor (void* bitmapData, size_t length, AvnPixelSize hotPixel, ComPtr<IAvnCursor>& retOut) override
    {
        @autoreleasepool
        {
            if(bitmapData == nullptr)
            {
                return E_POINTER;
            }
            
            NSData *imageData = [NSData dataWithBytes:bitmapData length:length];
            NSImage *image = [[NSImage alloc] initWithData:imageData];
            
            
            NSPoint hotSpot;
            hotSpot.x = hotPixel.Width;
            hotSpot.y = hotPixel.Height;
            
            retOut = comnew<Cursor>([[NSCursor new] initWithImage: image hotSpot: hotSpot]);
            
            return S_OK;
        }
    }
};

extern ComPtr<IAvnCursorFactory> CreateCursorFactory()
{
    return comnew<CursorFactory>();
}
