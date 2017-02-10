#include "pch.h"
#include "OpticalFlow.h"

using namespace UWPOpenCV;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation::Numerics;
using namespace Windows::Graphics;
using namespace Windows::Graphics::Imaging;

using namespace cv;
using namespace std;

TermCriteria termcrit(TermCriteria::COUNT | TermCriteria::EPS, 20, 0.03);

const int MAX_COUNT = 500;
const cv::Size subPixWinSize(10, 10);
const cv::Size winSize(31, 31);


template<typename T>
Microsoft::WRL::ComPtr<T> AsComPtr(Platform::Object^ object)
{
    Microsoft::WRL::ComPtr<T> p;
    reinterpret_cast<IUnknown*>(object)->QueryInterface(IID_PPV_ARGS(&p));
    return p;
}

OpticalFlow::OpticalFlow()
    : _initialized(false)
{
}

IVector<Windows::Foundation::Point>^ OpticalFlow::getInterestPoints()
{
    Vector<Windows::Foundation::Point>^ interestPoints = ref new Vector<Windows::Foundation::Point>();
    auto vec = points[1];
    for each (Point2f var in vec)
    {
        interestPoints->Append(Windows::Foundation::Point(var.x, var.y));
    }

    return interestPoints;
}


void OpticalFlow::initialize()
{
    // need to periodically call this as things go out of frame.
    _initialized = false;
}

void OpticalFlow::computeFrame(SoftwareBitmap^ softwareBitmap)
{
    // Convert the software bitmap into an opencv image
    try
    {
        SoftwareBitmap^ newBitmap = SoftwareBitmap::Convert(softwareBitmap, BitmapPixelFormat::Gray8);
        auto input = newBitmap->LockBuffer(BitmapBufferAccessMode::Read);
        int inputStride = input->GetPlaneDescription(0).Stride;
        int pixelWidth = softwareBitmap->PixelWidth;
        int pixelHeight = softwareBitmap->PixelHeight;

        if (gray.empty())
        {
            gray.create(pixelWidth, pixelHeight, CV_8UC1);
        }

        auto inputReference = input->CreateReference();

        byte* inputBytes;
        uint inputCapacity;

        AsComPtr<IMemoryBufferByteAccess>(inputReference)->GetBuffer(&inputBytes, &inputCapacity);


        for (int y = 0; y < pixelHeight; y++)
        {
            byte* inputRowBytes = inputBytes + y * inputStride;
            byte* greyRow = gray.ptr<byte>(y);

            while (pixelWidth--)
            {
                *greyRow++ = *inputRowBytes++;
            }

            pixelWidth = softwareBitmap->PixelWidth;
        }

        delete inputReference;
        delete input;
    }
    catch (std::exception)
    {
        return;
    }


    if (!_initialized)
    {
        goodFeaturesToTrack(gray, points[1], MAX_COUNT, 0.01, 10, Mat(), 3, 0, 0.04);
        cornerSubPix(gray, points[1], subPixWinSize, cv::Size(-1, -1), termcrit);
        _initialized = true;
    }
    else
    {
        vector<uchar> status;
        vector<float> err;
        calcOpticalFlowPyrLK(prevGray, gray, points[0], points[1], status, err, winSize, 3, termcrit, 0, 0.001);
    }

    std::swap(points[1], points[0]);
    cv::swap(prevGray, gray);
}

