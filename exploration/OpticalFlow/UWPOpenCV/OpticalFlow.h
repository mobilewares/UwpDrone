#pragma once

namespace UWPOpenCV
{
    public ref class OpticalFlow sealed
    {
        cv::Mat gray;
        cv::Mat prevGray;

        std::vector<cv::Point2f> points[2];

        bool _initialized;
    public:
        OpticalFlow();

        Windows::Foundation::Collections::IVector<Windows::Foundation::Point>^ getInterestPoints();

        void initialize();
        void computeFrame(Windows::Graphics::Imaging::SoftwareBitmap^ softwareBitmap);
    };
}
